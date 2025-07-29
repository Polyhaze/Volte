using Starscript;
using Volte.Systems.Starscript;

namespace Volte.Systems.Commands.Interaction;

public class VolteStarscriptAutocompleter : AbstractStarscriptAutocompleter
{
    public override StarscriptHypervisor Hypervisor { get; protected set; } = VolteStarscript.Hypervisor;
}