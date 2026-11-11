import { memo } from 'react';

import { usePieChartSize } from '../hooks';

import { CustomPieChart } from '@/components/PieChart';
import { TotalTimeChartLabel } from '@/components/TotalTimeChartLabel';
import { convertToChartData } from '@/helpers/convertToChartData';

import { employeeChartType } from './constants';
import { Wrapper } from './styles';
import { EmployeeListProps } from './types';

function EmployeeListComponent({
	chartData,
	generalDuration,
}: EmployeeListProps) {
	const pieChartSize = usePieChartSize();

	return (
		<Wrapper>
			<CustomPieChart
				data={convertToChartData(
					chartData?.[employeeChartType] && chartData[employeeChartType]
				)}
				size={pieChartSize}
				showAdditionalInfo={false}
			/>

			<TotalTimeChartLabel text={generalDuration[employeeChartType]} />
		</Wrapper>
	);
}

export const EmployeeList = memo(EmployeeListComponent);
