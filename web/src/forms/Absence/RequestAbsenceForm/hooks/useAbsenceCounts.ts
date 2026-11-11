import { useMemo } from 'react';

import { TrackingUnitType, UserAbsenceItem } from '@/types/absences';

interface UseAbsenceCountsProps {
	selectedAbsence: UserAbsenceItem | null;
	hoursValue: string | null;
	selectedDays: number | null;
}

export const useAbsenceCounts = ({
	selectedAbsence,
	hoursValue,
	selectedDays,
}: UseAbsenceCountsProps) => {
	const selectedAbsenceUnitCount = selectedAbsence?.trackingUnitCount ?? 0;

	const isHoursMode =
		selectedAbsence?.trackingUnitType === TrackingUnitType.Hours;
	const isZeroDaysSelected = selectedDays === 0;

	const selectedCount = useMemo(() => {
		if (isHoursMode) {
			if (isZeroDaysSelected) {
				return 0;
			}

			return Number(hoursValue) || 0;
		}

		return selectedDays;
	}, [isHoursMode, isZeroDaysSelected, hoursValue, selectedDays]);

	const remainingDays = useMemo(
		() => selectedAbsenceUnitCount - (selectedCount || 0),
		[selectedAbsenceUnitCount, selectedCount]
	);

	const isShowAvailable = useMemo(
		() =>
			selectedAbsence?.trackingUnitType === TrackingUnitType.Days &&
			selectedAbsence?.isAgreement,
		[selectedAbsence?.trackingUnitType, selectedAbsence?.isAgreement]
	);

	return {
		selectedAbsenceUnitCount,
		selectedCount,
		remainingDays,
		isShowAvailable,
		isZeroDaysSelected: isHoursMode && isZeroDaysSelected,
	};
};
