import { FormattedTime } from '@mms/mms-ui-library/components/FomattedTime';
import { Tooltip } from '@mms/mms-ui-library/components/Tooltip';
import { memo, useCallback, useMemo, useState } from 'react';
import { Cell, Pie, PieChart, ResponsiveContainer } from 'recharts';

import { ActiveShape } from './ActiveShape';
import {
	PIE_CHART_DEFAULT_INNER_RADIUS,
	PIE_CHART_DEFAULT_OUTER_RADIUS,
	PIE_CHART_SIZES,
} from './constants';
import { Wrapper, InfoWrapper, Name, PieChartWrapper } from './styles';
import { CustomPieChartProps } from './types';

function CustomPieChartInner({
	activeIndex,
	onPieEnter,
	data,
	size = 'medium',
	showAdditionalInfo = true,
	tooltipPosition = 'top',
	innerRadius = PIE_CHART_DEFAULT_INNER_RADIUS,
	outerRadius = PIE_CHART_DEFAULT_OUTER_RADIUS,
}: CustomPieChartProps) {
	const [name, setName] = useState('');
	const [time, setTime] = useState('');

	const handlePieEnter = useCallback(
		(pieChartProps: any, index: number) => {
			if (onPieEnter) {
				onPieEnter(pieChartProps ? index : undefined);
			}
		},
		[onPieEnter]
	);

	const memoizedData = useMemo(
		() =>
			data?.map(({ name: uniqName, color }, index) => (
				<Cell key={uniqName ?? index} fill={color} />
			)),
		[data]
	);

	const [hours, minutes] = time.split(':');

	return (
		<Wrapper
			tooltipPosition={tooltipPosition}
			width={PIE_CHART_SIZES[size]}
			data-testid="piechart"
		>
			{showAdditionalInfo && (
				<InfoWrapper
					tooltipPosition={tooltipPosition}
					chartWidth={PIE_CHART_SIZES[size]}
				>
					{(Boolean(activeIndex) || activeIndex === 0) && (
						<>
							<Tooltip position="bottom" text={name} useIfTextCut>
								<Name variant="l-600">{name}</Name>
							</Tooltip>

							<FormattedTime hours={hours} minutes={minutes} />
						</>
					)}
				</InfoWrapper>
			)}

			<PieChartWrapper
				width={PIE_CHART_SIZES[size]}
				height={PIE_CHART_SIZES[size]}
			>
				<ResponsiveContainer width="100%" height="100%">
					<PieChart>
						<Pie
							stroke="none"
							activeIndex={activeIndex}
							data={data}
							innerRadius={innerRadius}
							outerRadius={outerRadius}
							dataKey="value"
							activeShape={<ActiveShape setName={setName} setTime={setTime} />}
							onMouseEnter={handlePieEnter}
						>
							{memoizedData}
						</Pie>
					</PieChart>
				</ResponsiveContainer>
			</PieChartWrapper>
		</Wrapper>
	);
}

export const CustomPieChart = memo(CustomPieChartInner);
