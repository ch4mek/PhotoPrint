import { useMemo } from 'react';

import { UseGetAbsenceTypeTotalCountProps } from '../types';

import { initialAbsenceFilterValue } from '@/constants/absence';
import { removeLastLetterAccordingToAmount } from '@/helpers/removeLastLetterAccordingToAmount';

export const useGetAbsenceTypeTotalCount = ({
	absenceFilter,
	absences,
	totalUsed,
}: UseGetAbsenceTypeTotalCountProps) =>
	useMemo(() => {
		if (absenceFilter.id === initialAbsenceFilterValue.id) {
			return null;
		}

		const { trackingUnitType } = absences.find(
			({ name }) => name === absenceFilter.id
		) || {
			trackingUnitType: '',
		};

		const type = removeLastLetterAccordingToAmount(
			trackingUnitType.toLowerCase(),
			totalUsed || 0
		);

		return `${totalUsed || 0} ${type}`;
	}, [absences, absenceFilter, totalUsed]);
