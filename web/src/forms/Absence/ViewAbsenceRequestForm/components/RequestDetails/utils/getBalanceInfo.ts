import { ABSENCE_BALANCE, ABSENCE_REMAINING } from '@/constants/absence';
import { getCountLabelWithUnit } from '@/helpers/absences';
import { TrackingUnitType } from '@/types/absences';

export const getBalanceInfo = (
	remainingDays: number | null | undefined,
	balanceDays: number | null | undefined,
	balanceHours: number | null | undefined
) => {
	if (remainingDays != null) {
		return {
			count: remainingDays,
			label: getCountLabelWithUnit(TrackingUnitType.Days, ABSENCE_REMAINING),
		};
	}

	if (balanceDays != null) {
		return {
			count: balanceDays,
			label: getCountLabelWithUnit(TrackingUnitType.Days, ABSENCE_BALANCE),
		};
	}

	if (balanceHours != null) {
		return {
			count: balanceHours,
			label: getCountLabelWithUnit(TrackingUnitType.Hours, ABSENCE_BALANCE),
		};
	}

	return {
		count: 0,
		label: getCountLabelWithUnit(TrackingUnitType.Days, ABSENCE_BALANCE),
	};
};
