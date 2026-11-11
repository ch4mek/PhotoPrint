import { useEffect, useMemo, useState } from 'react';

import { convertDataToOptions } from '../utils/convertDataToOptions';

import { UserAbsenceItem } from '@/types/absences';

interface UseAbsenceSelectionProps {
	absences?: UserAbsenceItem[];
	initialAbsenceTypeId?: string | number;
}

export const useAbsenceSelection = ({
	absences,
	initialAbsenceTypeId,
}: UseAbsenceSelectionProps) => {
	const [selectedAbsence, setSelectedAbsence] =
		useState<UserAbsenceItem | null>(null);

	const absenceNamesOptions = useMemo(
		() => convertDataToOptions(absences),
		[absences]
	);

	const initialAbsenceName = useMemo(() => {
		if (initialAbsenceTypeId && absenceNamesOptions) {
			return (
				absenceNamesOptions.find(
					(item) => item.id.toString() === initialAbsenceTypeId.toString()
				) || null
			);
		}

		return null;
	}, [absenceNamesOptions, initialAbsenceTypeId]);

	useEffect(() => {
		if (absences && absences.length > 0) {
			let foundItem: UserAbsenceItem | undefined;

			if (initialAbsenceTypeId) {
				foundItem = absences.find((item) => {
					const itemId = item.inquiryTemplateId ?? item.agreementTemplateId;

					return itemId?.toString() === initialAbsenceTypeId.toString();
				});
			}

			const itemToSelect = foundItem || absences[0];
			setSelectedAbsence(itemToSelect);
		}
	}, [absences, initialAbsenceTypeId]);

	return {
		selectedAbsence,
		absenceNamesOptions,
		initialAbsenceName,
		setSelectedAbsence,
	};
};
