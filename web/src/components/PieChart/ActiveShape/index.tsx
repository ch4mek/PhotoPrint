import { memo, useEffect } from 'react';
import { Sector } from 'recharts';

import { minutesToHours } from '@/helpers/minutesToHours';

import type { ActiveShapeProps } from './types';

const increasingInnerRadiusOfOuterSector = 5;
const increasingOuterRadiusOfOuterSector = 8;

function ActiveShapeInner({
	cx,
	cy,
	innerRadius,
	outerRadius,
	startAngle,
	endAngle,
	fill,
	payload,
	setName,
	setTime,
}: ActiveShapeProps) {
	const userTime = `${minutesToHours(payload?.value as number)}`;

	useEffect(() => {
		setName(payload?.name as string);
		setTime(userTime);
	}, []);

	return (
		<g style={{ pointerEvents: 'none' }}>
			<Sector
				cx={cx}
				cy={cy}
				innerRadius={innerRadius}
				outerRadius={outerRadius}
				startAngle={startAngle}
				endAngle={endAngle}
				fill={fill}
			/>
			<Sector
				cx={cx}
				cy={cy}
				startAngle={startAngle}
				endAngle={endAngle}
				innerRadius={
					outerRadius
						? outerRadius + increasingInnerRadiusOfOuterSector
						: undefined
				}
				outerRadius={
					outerRadius
						? outerRadius + increasingOuterRadiusOfOuterSector
						: undefined
				}
				fill={fill}
			/>
		</g>
	);
}

export const ActiveShape = memo(ActiveShapeInner);
