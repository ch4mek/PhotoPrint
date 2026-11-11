import { Text } from '../styles';

import { Y_POSITION_OFFSET } from './constants';
import type { CustomizedYAxisTickProps } from './types';

export function CustomizedYAxisTick({ y, payload }: CustomizedYAxisTickProps) {
	return (
		<g transform={`translate(${0},${Number(y) + Y_POSITION_OFFSET})`}>
			<Text x={0} y={0} textAnchor="start">
				{`${payload?.value} h`}
			</Text>
		</g>
	);
}
