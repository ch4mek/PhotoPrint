import { useCallback, useContext, useMemo, useState } from 'react';

import { DEFAULT_ORDER_BY_FIELD } from '../EducationsGrid/constants';
import { useEducationGridColumns } from '../EducationsGrid/hooks/useEducationGridColumns';
import { useEducationsFilters } from '../EducationsGrid/hooks/useEducationsFilters';
import { useEducationsGridRows } from '../EducationsGrid/hooks/useEducationsGridRows';
import { EducationsFilterOptions } from '../EducationsGrid/types';

import { SortDirection } from '@/constants/sort';
import { useConfiguration } from '@/context/Configuration';
import { convertToSelectionModalOptions } from '@/helpers/convertToSelectionModalOptions';
import { createGridPagination } from '@/helpers/createGridPagination';
import { useTableState } from '@/pages/hooks/useTableState';
import { PersonalDataContext } from '@/pages/TeamBoardUser/context/personalDataContext';
import { useGetEducations } from '@/queries/educations/useGetEducations';
import { EducationsResponse } from '@/types/educations';

export const useEducationsGridState = () => {
	const { filters, setFilters } = useEducationsFilters();
	const { userId } = useContext(PersonalDataContext);
	const [isFormVisible, setIsFormVisible] = useState(false);
	const [chosenEducation, setChosenEducation] =
		useState<EducationsResponse | null>(null);

	const {
		persistentValues: { educationLevelTypes, studyFormTypes },
	} = useConfiguration();

	const {
		rowsPerPage,
		page,
		setPage,
		sortColumns,
		order,
		orderBy,
		handleSortColumnChange,
		handleOnPerPageChange,
	} = useTableState({
		initialOrder: SortDirection.DESC,
		initialOrderBy: DEFAULT_ORDER_BY_FIELD,
		resetPageDeps: [filters],
	});

	const { data: userEducations, isLoading } = useGetEducations({
		userId: userId!,
		filters,
		order,
		orderBy,
		page,
		rowsPerPage,
		allFilterOptions: {
			educationLevels: educationLevelTypes,
			studyForms: studyFormTypes,
		},
	});

	const handleItemEditClick = useCallback(
		(educationId: number) => () => {
			if (userEducations && educationId) {
				setChosenEducation(
					userEducations.items.find(({ id }) => id === educationId) ?? null
				);
				setIsFormVisible(true);
			}
		},
		[userEducations, setChosenEducation, setIsFormVisible]
	);

	const rows = useEducationsGridRows({
		educations: userEducations?.items ?? [],
		educationLevelTypes: educationLevelTypes ?? [],
		studyFormTypes: studyFormTypes ?? [],
		handleItemEditClick,
	});

	const filterOptions: EducationsFilterOptions = useMemo(
		() => ({
			educationLevels: convertToSelectionModalOptions(
				educationLevelTypes ?? [],
				'description'
			),
			studyForms: convertToSelectionModalOptions(
				studyFormTypes ?? [],
				'description'
			),
		}),
		[educationLevelTypes, studyFormTypes]
	);

	const columns = useEducationGridColumns({
		filters,
		setFilters,
		options: filterOptions,
	});

	const educationsPagination = useMemo(
		() =>
			createGridPagination(
				rows.length,
				page,
				setPage,
				rowsPerPage,
				handleOnPerPageChange,
				userEducations?.total
			),
		[rows, page, rowsPerPage, userEducations?.total]
	);

	const handlePopupToggle = () => {
		setIsFormVisible((prevVisible) => !prevVisible);
		setChosenEducation(null);
	};

	return {
		rows,
		columns,
		educationsPagination,
		handleSortColumnChange,
		sortColumns,
		isLoading,
		handlePopupToggle,
		isFormVisible,
		chosenEducation,
	};
};
