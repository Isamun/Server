﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.18052
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Demo.Client.Properties {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "4.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class Resources {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Resources() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("Demo.Client.Properties.Resources", typeof(Resources).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   Overrides the current thread's CurrentUICulture property for all
        ///   resource lookups using this strongly typed resource class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to {
        ///	&quot;server&quot; : {
        ///		&quot;ip&quot; : &quot;127.0.0.1&quot;,
        ///		&quot;port&quot;: 1337
        ///	}
        ///}.
        /// </summary>
        internal static string default_config {
            get {
                return ResourceManager.GetString("default_config", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to {
        ///
        ///  &quot;MessageID&quot;: 1301,
        ///
        ///  &quot;MessageDescription&quot;: &quot;Client please, Take this state I have boundled in this event and display it to the operator&quot;,
        ///
        ///  &quot;MessageType&quot;: &quot;Command&quot;,
        ///
        ///  &quot;Source&quot;: &quot;TestStand&quot;,
        ///
        ///  &quot;Data&quot;: {
        ///
        ///    &quot;CurrentStep&quot;: null,
        ///
        ///    &quot;CurrentSequence&quot;: {
        ///
        ///      &quot;StepList&quot;: [
        ///
        ///        {
        ///
        ///          &quot;SequenceRef&quot;: null,
        ///
        ///          &quot;Type&quot;: &quot;PassFailTest&quot;,
        ///
        ///          &quot;Status&quot;: 1337,
        ///
        ///          &quot;Name&quot;: &quot;loltest&quot;,
        ///
        ///          &quot;Description&quot;: &quot;Pass/Fail Test&quot;,
        ///
        ///          &quot;Settin [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string MockUpSequenceFileLoadedPDU {
            get {
                return ResourceManager.GetString("MockUpSequenceFileLoadedPDU", resourceCulture);
            }
        }
    }
}
