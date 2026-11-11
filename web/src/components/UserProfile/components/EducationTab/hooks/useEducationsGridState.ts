import { useMemo } from 'react';

import { DEFAULT_ORDER_BY_FIELD } from '../EducationsGrid/constants';
import { useEducationGridColumns } from '../EducationsGrid/hooks/useEducationGridColumns';
import { useEducationsFilters } from '../EducationsGrid/hooks/useEducationsFilters';
import { useEducationsGridRows } from '../EducationsGrid/hooks/useEducationsGridRows';
import { EducationsFilterOptions } from '../EducationsGrid/types';

import { DEFAULT_ROWS_PER_PAGE } from '@/constants/pagination';
import { SortDirection } from '@/constants/sort';
import { useAuth } from '@/context/AuthContext';
import { useConfiguration } from '@/context/Configuration';
import { convertToSelectionModalOptions } from '@/helpers/convertToSelectionModalOptions';
import { createGridPagination } from '@/helpers/createGridPagination';
import { useTableState } from '@/pages/hooks/useTableState';
import { useGetEducations } from '@/queries/educations/useGetEducations';

export const useEducationsGridState = () => {
	const { filters, setFilters } = useEducationsFilters();
	const { id: userId } = useAuth();

	const {
		persistentValues: { educationLevelTypes, studyFormTypes },
	} = useConfiguration();

	const {
		rowsPerPage,
		sortColumns,
		page,
		setPage,
		order,
		orderBy,
		handleSortColumnChange,
		handleOnPerPageChange,
	} = useTableState({
		initialRowsPerPage: Number(DEFAULT_ROWS_PER_PAGE),
		initialOrder: SortDirection.DESC,
		initialOrderBy: DEFAULT_ORDER_BY_FIELD,
		resetPageDeps: [filters],
	});

	const { data: userEducations, isLoading } = useGetEducations({
		userId,
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

	const rows = useEducationsGridRows({
		educations: userEducations?.items ?? [],
		educationLevelTypes: educationLevelTypes ?? [],
		studyFormTypes: studyFormTypes ?? [],
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
		[
			rows.length,
			page,
			rowsPerPage,
			userEducations?.total,
			setPage,
			handleOnPerPageChange,
		]
	);

	return {
		rows,
		columns,
		educationsPagination,
		handleSortColumnChange,
		sortColumns,
		isLoading,
	};
};
