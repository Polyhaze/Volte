using Starscript;

namespace Volte.Interactions.Commands;

public class VolteStarscriptAutocompleter : AbstractStarscriptAutocompleter
{
    public override StarscriptHypervisor Hypervisor { get; protected set; } = VolteStarscript.Hypervisor;
}