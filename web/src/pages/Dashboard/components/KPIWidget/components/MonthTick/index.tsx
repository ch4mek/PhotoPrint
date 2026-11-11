import {
	MONTH_NAME_OFFSET_Y,
	PERCENT_OFFSET_Y,
	PERCENT_SUFFIX,
} from './constants';
import { MonthTickProps } from './types';
import { getPercentColor } from './utils/getPercentColor';

export function MonthTick({ x, y, index, data, theme }: MonthTickProps) {
	const item = data[index];
	const percent = item?.percent ?? null;

	const monthTextProps = {
		textAnchor: 'middle' as const,
		fontSize: theme.typography.body['s-400'].fontSize,
		fontWeight: theme.typography.body['s-400'].fontWeight,
		fill: theme.palette['text-gray'],
	};

	const percentTextProps = {
		textAnchor: 'middle' as const,
		fontSize: theme.typography.body['xs-400'].fontSize,
		fontWeight: theme.typography.body['xs-400'].fontWeight,
		fill: theme.palette['text-gray'],
	};

	return (
		<g transform={`translate(${x},${y})`}>
			<text dy={MONTH_NAME_OFFSET_Y} {...monthTextProps}>
				{item?.monthName}
			</text>
			{percent !== null && (
				<text
					{...percentTextProps}
					dy={PERCENT_OFFSET_Y}
					fill={getPercentColor(item.color, theme)}
				>
					{percent}
					{PERCENT_SUFFIX}
				</text>
			)}
		</g>
	);
}
