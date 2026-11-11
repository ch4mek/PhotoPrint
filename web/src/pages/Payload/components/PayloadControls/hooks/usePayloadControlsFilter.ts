import { useCallback, useEffect, useMemo, useState } from 'react';

import { CURRENT_YEAR } from '@/constants/date';
import {
	INITIAL_PAGE_NUMBER,
	MAX_ROWS_PER_PAGE_INT,
} from '@/constants/pagination';
import { useAuth } from '@/context/AuthContext';
import { usePayloadContext } from '@/context/PayloadContext';
import { getFilterYears } from '@/helpers/getFilterYears';
import {
	INITIAL_SPECIALIZATION_TITLE,
	MINIMAL_HARD_SKILLS_COUNT_FOR_SPECIALIZATION,
} from '@/pages/Payload/contants';
import { useSpecializations } from '@/queries/settings/useSpecializations';

export function usePayloadControlsFilter() {
	const { departmentId } = useAuth();

	const { data } = useSpecializations(
		INITIAL_PAGE_NUMBER,
		MAX_ROWS_PER_PAGE_INT,
		departmentId,
		MINIMAL_HARD_SKILLS_COUNT_FOR_SPECIALIZATION
	);

	const { year, setYear, setSpecializationTitle, specializationTitle } =
		usePayloadContext();

	const [specializationOptions, setSpecializationOptions] = useState<string[]>([
		INITIAL_SPECIALIZATION_TITLE,
	]);

	const yearsOptions = useMemo(() => getFilterYears(), []);

	const handleYearChange = useCallback((value: number | null) => {
		if (value) {
			setYear(value);
		}
	}, []);

	const handleSpecializationChange = useCallback((value: string | null) => {
		if (value) {
			setSpecializationTitle(value);
		}
	}, []);

	const handleResetFilter = useCallback(() => {
		setYear(CURRENT_YEAR);
		setSpecializationTitle(INITIAL_SPECIALIZATION_TITLE);
	}, []);

	const isResetFilterDisabled = useMemo(
		() =>
			Number(year) === CURRENT_YEAR &&
			specializationTitle === INITIAL_SPECIALIZATION_TITLE,
		[year, specializationTitle]
	);

	useEffect(() => {
		if (data?.items) {
			setSpecializationOptions([
				INITIAL_SPECIALIZATION_TITLE,
				...data.items.map((spec) => spec.name),
			]);
		}
	}, [data]);

	return {
		specializationOptions,
		yearsOptions,
		handleYearChange,
		handleSpecializationChange,
		handleResetFilter,
		isResetFilterDisabled,
		year,
		specializationTitle,
	};
}
