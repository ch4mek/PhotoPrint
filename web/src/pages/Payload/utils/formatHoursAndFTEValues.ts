import { TotalMonth, WorkLoad } from '../types';

import { formatToFixedPrecision } from '@/helpers/formatToFixedPrecision';

export const formatHoursAndFTEValues = <T extends WorkLoad>({
	actualFte,
	plannedFte,
	plannedWorkloadInHours,
	requiredHours,
	trackedBillableInHours,
	...rest
}: T) => ({
	...rest,
	actualFte: formatToFixedPrecision(actualFte),
	plannedFte: formatToFixedPrecision(plannedFte),
	requiredHours: formatToFixedPrecision(requiredHours),
	trackedBillableInHours: formatToFixedPrecision(trackedBillableInHours),
	...(plannedWorkloadInHours != null && {
		plannedWorkloadInHours: formatToFixedPrecision(plannedWorkloadInHours),
	}),
});

export const formatTotalHoursAndFTEValues = ({
	totalActualFte,
	totalFactBillableInHours,
	totalKpi,
	totalPlannedFte,
	totalPlannedWorkloadInHours,
	...rest
}: TotalMonth) => ({
	...rest,
	totalActualFte: formatToFixedPrecision(totalActualFte),
	totalPlannedFte: formatToFixedPrecision(totalPlannedFte),
	totalFactBillableInHours: formatToFixedPrecision(totalFactBillableInHours),
	totalPlannedWorkloadInHours: formatToFixedPrecision(
		totalPlannedWorkloadInHours
	),
	totalKpi: Math.round(totalKpi),
});
