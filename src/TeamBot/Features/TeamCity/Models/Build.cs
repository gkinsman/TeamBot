using System.Collections.Generic;
using Newtonsoft.Json;

namespace TeamBot.Features.TeamCity.Models
{
    public class BuildType
    {   
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }

        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        [JsonProperty(PropertyName = "projectName")]
        public string ProjectName { get; set; }

        [JsonProperty(PropertyName = "projectId")]
        public string ProjectId { get; set; }

        [JsonProperty(PropertyName = "href")]
        public string Link { get; set; }

        [JsonProperty(PropertyName = "webUrl")]
        public string WebUrl { get; set; }
    }

    public class Triggered
    {
        [JsonProperty(PropertyName = "type")]
        public string Type { get; set; }
        
        [JsonProperty(PropertyName = "details")]
        public string Details { get; set; }

        [JsonProperty(PropertyName = "date")]
        public string Date { get; set; }
    }

    public class LastChanges
    {
        [JsonProperty(PropertyName = "count")]
        public int Count { get; set; }

        [JsonProperty(PropertyName = "change")]
        public IEnumerable<Change> Change { get; set; }
    }

    public class Change
    {
        [JsonProperty(PropertyName = "id")]
        public int Id {get; set;}
        
        [JsonProperty(PropertyName = "version")]
        public string Version {get; set;}
        
        [JsonProperty(PropertyName = "username")]
        public string Username {get; set;}
        
        [JsonProperty(PropertyName = "date")]
        public string Date {get; set;}
        
        [JsonProperty(PropertyName = "href")]
        public string Links {get; set;}

        [JsonProperty(PropertyName = "webLink")]
        public string WebLink {get; set;}    
    }

    public class Changes
    {
        [JsonProperty(PropertyName = "count")]
        public int Count { get; set; }

        [JsonProperty(PropertyName = "href")]
        public string Link { get; set; }
    }

    public class Revisions
    {
        [JsonProperty(PropertyName = "Revision")]
        public Revision Revision { get; set; }
    }

    public class Revision
    {
        [JsonProperty(PropertyName = "version")]
        public string Version { get; set; }

        [JsonProperty(PropertyName = "vcs-root-instance")]
        public VcsRootInstance VcsRootInstance { get; set; } 
    }

    public class Href
    {
        [JsonProperty(PropertyName = "href")]
        public string Link { get; set; }
    }

    public class Agent
    {
        [JsonProperty(PropertyName = "id")]
        public int Id { get; set; }

        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        [JsonProperty(PropertyName = "typeId")]
        public int TypeId { get; set; }

        [JsonProperty(PropertyName = "href")]
        public string Link { get; set; }
    }

    public class VcsRootInstance
    {
        [JsonProperty(PropertyName = "id")]
        public int Id { get; set; }

        [JsonProperty(PropertyName = "vcs-root-id")]
        public string VcsRootId { get; set; }
        
        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }
        
        [JsonProperty(PropertyName = "href")]
        public string Link { get; set; }
    }

    public class Properties
    {
        [JsonProperty(PropertyName = "count")]
        public int Count { get; set; }

        [JsonProperty(PropertyName = "property")]
        public IEnumerable<Property> Property { get; set; }
    }

    public class Property
    {
        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        [JsonProperty(PropertyName = "value")]
        public string Value { get; set; }
    }

    public class Build
    {
        [JsonProperty(PropertyName = "id")]
        public int Id { get; set; }

        [JsonProperty(PropertyName = "buildTypeId")]
        public string BuildTypeId { get; set; }

        [JsonProperty(PropertyName = "number")]
        public string Number { get; set; }

        [JsonProperty(PropertyName = "status")]
        public string Status { get; set; }

        [JsonProperty(PropertyName = "state")]
        public string State { get; set; }

        [JsonProperty(PropertyName = "href")]
        public string Link { get; set; }

        [JsonProperty(PropertyName = "webUrl")]
        public string WebUrl { get; set; }
    }

    public class BuildExtended : Build
    {
        [JsonProperty(PropertyName = "statusText")]
        public string StatusText { get; set; }
        
        [JsonProperty(PropertyName = "buildType")]
        public BuildType BuildType { get; set; }

        [JsonProperty(PropertyName = "tags")]
        public Tags Tags { get; set; }
        
        [JsonProperty(PropertyName = "queuedDate")]
        public string QueuedDate { get; set; }
        
        [JsonProperty(PropertyName = "startDate")]
        public string StartDate { get; set; }
        
        [JsonProperty(PropertyName = "finishDate")]
        public string FinishDate { get; set; }
        
        [JsonProperty(PropertyName = "triggered")]
        public Triggered Triggered { get; set; }

        [JsonProperty(PropertyName = "lastChanges")]
        public LastChanges LastChanges { get; set; }

        [JsonProperty(PropertyName = "changes")]
        public Changes Changes { get; set; }
        
        [JsonProperty(PropertyName = "Revision")]
        public Revisions Revisions { get; set; }

        [JsonProperty(PropertyName = "Agent")]
        public Agent Agent { get; set; }
        
        [JsonProperty(PropertyName = "artifacts")]
        public Href Artifacts { get; set; }

        [JsonProperty(PropertyName = "relatedIssues")]
        public Href RelatedIssues { get; set; }
        
        [JsonProperty(PropertyName = "properties")]
        public Properties Properties { get; set; }

        [JsonProperty(PropertyName = "statistics")]
        public Href Statistics { get; set; }        
        
        [JsonProperty(PropertyName = "snapshot-dependencies")]
        public SnapshotDependencies SnapshotDependencies { get; set; }
    }

    public class Tags
    {
        [JsonProperty(PropertyName = "tag")]
        public IEnumerable<string> Tag { get; set; }
    }

    public class SnapshotDependencies
    {
        [JsonProperty(PropertyName = "count")]
        public int Count { get; set; }

        [JsonProperty(PropertyName = "build")]
        public IEnumerable<Build> Build { get; set; } 
    }
}