using Backend.Application.Services;
using Backend.Domain.Entities;
using Backend.Domain.Enums;
using Backend.Domain.ValueObjects;
using System.Collections.Concurrent;

namespace Backend.Tests.Tests
{
    [TestFixture]
    public class TaskQueueServiceTests
    {
        private TaskQueueService _service;

        [SetUp]
        public void Setup()
        {
            _service = new TaskQueueService();
        }

        [Test]
        public void CreateTask_ShouldReturnFailure_WhenMetadataIsNull()
        {
            var result = _service.CreateTask(null);

            Assert.IsFalse(result.IsSuccess);
            Assert.That(result.Error, Is.EqualTo("Metadata is required"));
        }

        [Test]
        public void CreateTask_ShouldReturnFailure_WhenFilePathIsEmpty()
        {
            var metadata = new TaskMetadata
            {
                SoundPath = "",
                Author = "author",
                Title = "title"
            };

            var result = _service.CreateTask(metadata);

            Assert.IsFalse(result.IsSuccess);
        }

        [Test]
        public void CreateTask_ShouldReturnFailure_WhenAuthorIsEmpty()
        {
            var metadata = new TaskMetadata
            {
                SoundPath = "path",
                Author = "",
                Title = "title"
            };

            var result = _service.CreateTask(metadata);

            Assert.IsFalse(result.IsSuccess);
        }

        [Test]
        public void CreateTask_ShouldReturnFailure_WhenTitleIsEmpty()
        {
            var metadata = new TaskMetadata
            {
                SoundPath = "path",
                Author = "author",
                Title = ""
            };

            var result = _service.CreateTask(metadata);

            Assert.IsFalse(result.IsSuccess);
        }

        [Test]
        public void CreateTask_ShouldReturnSuccess_WhenMetadataIsValid()
        {
            var metadata = new TaskMetadata
            {
                SoundPath = "path",
                Author = "author",
                Title = "title"
            };

            var result = _service.CreateTask(metadata);

            Assert.IsTrue(result.IsSuccess);
            Assert.NotNull(result.Value);
            Assert.That(result.Value.Id, Is.Not.EqualTo(Guid.Empty));
        }

        [Test]
        public void DeleteTask_ShouldReturnFalse_WhenTaskDoesNotExist()
        {
            var result = _service.DeleteTask(Guid.NewGuid());

            Assert.IsFalse(result);
        }

        [Test]
        public void DeleteTask_ShouldReturnTrue_WhenTaskExists()
        {
            var metadata = new TaskMetadata
            {
                SoundPath = "path",
                Author = "author",
                Title = "title"
            };

            var createdResult = _service.CreateTask(metadata);

            Assert.IsTrue(createdResult.IsSuccess);

            var created = createdResult.Value;

            var result = _service.DeleteTask(created.Id);

            Assert.IsTrue(result);
        }

        [Test]
        public void GetTask_ShouldReturnFailure_WhenTaskDoesNotExist()
        {
            var result = _service.GetTask(Guid.NewGuid());

            Assert.IsFalse(result.IsSuccess);
        }

        [Test]
        public void GetTask_ShouldReturnSuccess_WhenTaskExists()
        {
            var metadata = new TaskMetadata
            {
                SoundPath = "path",
                Author = "author",
                Title = "title"
            };

            var createdResult = _service.CreateTask(metadata);

            Assert.IsTrue(createdResult.IsSuccess);

            var created = createdResult.Value;

            var result = _service.GetTask(created.Id);

            Assert.IsTrue(result.IsSuccess);
            Assert.That(result.Value.Id, Is.EqualTo(created.Id));
        }
        [Test]
        public void TryGetAndLockPendingTask_ShouldReturnOnlyOneTask_WhenCalledConcurrently()
        {
            for (int i = 0; i < 50; i++)
            {
                _service.CreateTask(new TaskMetadata
                {
                    SoundPath = "path",
                    Author = "author",
                    Title = "title"
                });
            }

            var results = new ConcurrentBag<TaskItem>();
            var tasks = new List<Task>();

            for (int i = 0; i < 10; i++)
            {
                tasks.Add(Task.Run(() =>
                {
                    while (_service.TryGetAndLockPendingTask(out var task))
                    {
                        if (task != null)
                        {
                            results.Add(task);
                        }
                    }
                }));
            }

            Task.WaitAll(tasks.ToArray());

            var distinctIds = results.Select(x => x.Id).Distinct().Count();

            Assert.That(distinctIds, Is.EqualTo(results.Count));
        }

        [Test]
        public void TryGetAndLockPendingTask_ShouldNotReturnSameTaskTwice()
        {
            var metadata = new TaskMetadata
            {
                SoundPath = "path",
                Author = "author",
                Title = "title"
            };

            var created = _service.CreateTask(metadata).Value;

            var first = _service.TryGetAndLockPendingTask(out var task1);
            var second = _service.TryGetAndLockPendingTask(out var task2);

            Assert.IsTrue(first);
            Assert.IsFalse(second || task2 != null);
        }

        [Test]
        public void CreateTask_ShouldBeThreadSafe_WhenCalledConcurrently()
        {
            var bag = new ConcurrentBag<Guid>();

            Parallel.For(0, 100, _ =>
            {
                var result = _service.CreateTask(new TaskMetadata
                {
                    SoundPath = "path",
                    Author = "author",
                    Title = "title"
                });

                if (result.IsSuccess)
                {
                    bag.Add(result.Value.Id);
                }
            });

            Assert.That(bag.Count, Is.EqualTo(100));
            Assert.That(bag.Distinct().Count(), Is.EqualTo(100));
        }

        [Test]
        public void TryGetAndLockPendingTask_ShouldEventuallyConsumeAllTasks()
        {
            for (int i = 0; i < 20; i++)
            {
                _service.CreateTask(new TaskMetadata
                {
                    SoundPath = "path",
                    Author = "author",
                    Title = "title"
                });
            }

            var processed = 0;

            while (_service.TryGetAndLockPendingTask(out var task))
            {
                if (task != null)
                    processed++;
            }

            Assert.That(processed, Is.EqualTo(20));
        }

        [Test]
        public void Update_ShouldReplaceTask_WhenCalled()
        {
            var created = _service.CreateTask(new TaskMetadata
            {
                SoundPath = "path",
                Author = "author",
                Title = "title"
            }).Value;

            created.Status = Status.Processing;
            _service.Update(created);

            var result = _service.GetTask(created.Id);

            Assert.IsTrue(result.IsSuccess);
            Assert.That(result.Value.Status, Is.EqualTo(Status.Processing));
        }
    }
}