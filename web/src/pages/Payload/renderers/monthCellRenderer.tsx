import { useCallback, useMemo } from 'react';

import { PERIOD } from '@/constants/date';
import { isCurrentYearMonth } from '@/helpers/date';
import { WorkloadCell } from '@/pages/Payload/components/WorkloadCell';
import { MonthCellWrapper } from '@/pages/Payload/styles';
import { HoursDetail, PayloadGridRow } from '@/pages/Payload/types';
import { getNewCellHeight } from '@/pages/Payload/utils/getNewCellHeight';
import { getProjectsWithActiveAssigment } from '@/pages/Payload/utils/getProjectsWithActiveAssigment';
import { RenderCellPropsWithHoverIndex } from '@/types/columnHoverTypes';

export const MonthCellRenderer = ({
	row,
	column: { key, hoverIndex },
	onRowChange,
}: RenderCellPropsWithHoverIndex<PayloadGridRow, PayloadGridRow>) => {
	const info = row[key.toLowerCase() as keyof PayloadGridRow] as HoursDetail;

	const {
		monthOfYear,
		projectDetails,
		hasActivity = false,
		...summaryWorkloads
	} = info;
	const { expanded, id, projects, year, isLastRow, disabled } = row;

	const sortedProjectDetails = useMemo(
		() =>
			[...projectDetails].sort(
				(firstProject, secondProject) =>
					firstProject.projectId - secondProject.projectId
			),
		[projectDetails]
	);

	const handlePlannedWorkloadChange = useCallback(
		(newPlannedWorkload: number) => {
			const monthName = PERIOD[
				monthOfYear - 1
			].toLowerCase() as keyof PayloadGridRow;
			onRowChange({
				...row,
				[monthName]: {
					...(row[monthName] as HoursDetail),
					plannedFte: newPlannedWorkload,
					isInputActive: false,
				},
			});
		},
		[monthOfYear, row, onRowChange]
	);

	const isActiveMonth = isCurrentYearMonth(monthOfYear, year);

	const projectWithActiveAssigment = useMemo(
		() => getProjectsWithActiveAssigment(projects),
		[projects]
	);

	const height = getNewCellHeight(
		false,
		projects.length,
		projectWithActiveAssigment.length
	);

	return (
		<MonthCellWrapper
			data-hovercolindex={hoverIndex}
			data-activemonth={isActiveMonth}
			data-expanded={expanded}
			data-userid={id}
		>
			<WorkloadCell
				{...{ hasActivity, disabled }}
				isLastRow={isLastRow}
				workloads={summaryWorkloads}
				options={{
					needBackground: true,
					newHeight: height,
					isExpanded: expanded,
				}}
				handleRowChange={handlePlannedWorkloadChange}
				userId={id}
				month={monthOfYear}
				year={year}
			/>
			{expanded &&
				sortedProjectDetails.map(
					({
						projectId,
						actualFte,
						plannedFte,
						requiredHours,
						trackedBillableInHours,
						plannedWorkloadInHours,
					}) => (
						<WorkloadCell
							{...{ hasActivity, disabled }}
							key={projectId}
							projectId={projectId}
							userId={id}
							workloads={{
								actualFte,
								plannedFte,
								requiredHours,
								trackedBillableInHours,
								plannedWorkloadInHours,
							}}
							options={{
								isSubRows: true,
								needBackground: true,
								isExpanded: true,
							}}
							month={monthOfYear}
							year={year}
						/>
					)
				)}
		</MonthCellWrapper>
	);
};
