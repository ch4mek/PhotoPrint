import { TextLineHours, TextLinePeriod, Wrapper } from './styles';
import { BaseCustomizedTooltipProps } from './types';

export function ChartCustomizedTooltip({
	arrowPlacement,
	headerText,
	mainText,
	ref,
}: BaseCustomizedTooltipProps) {
	return (
		<Wrapper {...{ ref, arrowPlacement }}>
			<TextLinePeriod>{headerText}</TextLinePeriod>
			<TextLineHours>{mainText}</TextLineHours>
		</Wrapper>
	);
}
