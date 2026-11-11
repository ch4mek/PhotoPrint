import { useHasScreenWidth } from '@mms/mms-ui-library/hooks/useHasScreenWidth';
import { breakpoints } from '@mms/mms-ui-library/theme';
import { useMemo } from 'react';

import { Text } from './styles';
import type { CustomizedXAxisTickProps } from './types';

const { desktopS } = breakpoints;

export function CustomizedXAxisTick({
	x,
	y,
	index,
	values,
	payload,
}: CustomizedXAxisTickProps) {
	const isDesktopSWidth = useHasScreenWidth(desktopS);

	const firstTextLine = useMemo(() => {
		if (values) {
			return values[index as number].name.date;
		}

		return isDesktopSWidth ? payload?.value : payload?.value.slice(0, 3);
	}, [values, payload]);

	return (
		<g transform={`translate(${x},${y})`}>
			<Text x={0} y={0}>
				<tspan textAnchor="middle" x="0">
					{firstTextLine}
				</tspan>

				{values && (
					<tspan textAnchor="middle" x="0" dy="18">
						{values[index as number].name.weekDay.slice(0, 3)}
					</tspan>
				)}
			</Text>
		</g>
	);
}
