using System;
using System.Threading.Tasks;
using ConferencePlanner.GraphQL;
using ConferencePlanner.GraphQL.Attendees;
using ConferencePlanner.GraphQL.Data;
using ConferencePlanner.GraphQL.Sessions;
using ConferencePlanner.GraphQL.Speakers;
using ConferencePlanner.GraphQL.Tracks;
using ConferencePlanner.GraphQL.Types;
using HotChocolate;
using HotChocolate.Execution;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Snapshooter.Xunit;
using Xunit;


namespace GraphQL.Tests
{
    public class AttendeeTests
    {
        [Fact]
        public void Attendee_Schema_Changed()
        {
            var services = new ServiceCollection();
            services.AddDbContextPool<ApplicationDbContext>(
                options => options.UseInMemoryDatabase("Data Source=conferences.db"));

             SchemaBuilder.New()
                .AddServices(services.BuildServiceProvider())
                .AddQueryType(d => d.Name("Query"))
                    .AddType<AttendeeQueries>()
                .AddMutationType(d => d.Name("Mutation"))
                    .AddType<AttendeeMutations>()
                .AddType<AttendeeType>()
                .AddType<SessionType>()
                .AddType<SpeakerType>()
                .AddType<TrackType>()
                .EnableRelaySupport()
                .Create()
                .ToString()
                .MatchSnapshot();
        }
        
        [Fact]
        public async Task RegisterAttendee()
        {
            // arrange
            var services = new ServiceCollection();
            services.AddDbContextPool<ApplicationDbContext>(
                options => options.UseInMemoryDatabase("Data Source=conferences.db"));

            IQueryExecutor executor = SchemaBuilder.New()
                .AddServices(services.BuildServiceProvider())
                .AddQueryType(d => d.Name("Query"))
                    .AddType<AttendeeQueries>()
                .AddMutationType(d => d.Name("Mutation"))
                    .AddType<AttendeeMutations>()
                .AddType<AttendeeType>()
                .AddType<SessionType>()
                .AddType<SpeakerType>()
                .AddType<TrackType>()
                .EnableRelaySupport()
                .Create()
                .MakeExecutable();
            
            // act
            IExecutionResult result = await executor.ExecuteAsync(@"
                mutation RegisterAttendee {
                    registerAttendee(
                        input: {
                            emailAddress: ""michael@chillicream.com""
                                firstName: ""michael""
                                lastName: ""staib""
                                userName: ""michael3""
                            }) 
                    {
                        attendee {
                            id
                        }
                    }
                }");
            
            // assert
            result.MatchSnapshot();
        }
    }
}
