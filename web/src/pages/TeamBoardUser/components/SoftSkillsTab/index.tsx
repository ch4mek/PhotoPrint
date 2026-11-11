import { useCallback, useContext, useMemo } from 'react';

import { PersonalDataContext } from '../../context/personalDataContext';

import { Loader } from '@/components/Loader';
import { TabHeader } from '@/components/TabHeader';
import { TabTableWrapper } from '@/components/TabWrapper';
import { YYYY_MM_DD_DAYJS_DATE_FORMAT } from '@/constants/date';
import { DEFAULT_ROWS_PER_PAGE } from '@/constants/pagination';
import { SortDirection } from '@/constants/sort';
import { SeniorityForm } from '@/forms/TeamboardForm/SeniorityForm';
import { SeniorityFormsValues } from '@/forms/TeamboardForm/SeniorityForm/types';
import { areDatesEqual } from '@/helpers/areValuesEqual';
import { createGridPagination } from '@/helpers/createGridPagination';
import dayjs from '@/lib/dayjs';
import { useShowModal } from '@/pages/hooks/useShowModal';
import { useTableState } from '@/pages/hooks/useTableState';
import { SoftSkillsTable } from '@/pages/TeamBoardUser/components/SoftSkillsTab/components/SoftSkillsTable';
import { useGetAllSnapshotsDates } from '@/pages/TeamBoardUser/components/SoftSkillsTab/hooks/useGetAllSnapshotsDates';
import { useSoftSkillsFilters } from '@/pages/TeamBoardUser/components/SoftSkillsTab/hooks/useSoftSkillsFilters';
import { useSoftSkillsMutations } from '@/pages/TeamBoardUser/components/SoftSkillsTab/hooks/useSoftSkillsMutations';
import { useSoftSkillsTableRows } from '@/pages/TeamBoardUser/components/SoftSkillsTab/hooks/useSoftSkillsTableRows';
import { convertDataForRequest } from '@/pages/TeamBoardUser/components/SoftSkillsTab/utils/convertDataForRequest';
import { useUserSoftSkills } from '@/queries/softSkills/useUserSoftSkills';

import { HEADER_BUTTON_TITLE, SOFT_SKILLS_COLUMNS_KEY } from './constants';
import { useSoftSkillsTableColumns } from './hooks/useSoftSkillsTableColumns';

export function SoftSkillsTab() {
	const { handleOpen, isOpen, handleClose, id } = useShowModal();

	const { filters, setFilters } = useSoftSkillsFilters();

	const {
		rowsPerPage,
		page,
		setPage,
		order,
		orderBy,
		handleSortColumnChange,
		handleOnPerPageChange,
		sortColumns,
	} = useTableState({
		initialRowsPerPage: Number(DEFAULT_ROWS_PER_PAGE),
		initialOrder: SortDirection.DESC,
		initialOrderBy: SOFT_SKILLS_COLUMNS_KEY.softSkillId,
		resetPageDeps: [filters],
	});

	const { userId } = useContext(PersonalDataContext);

	const { data, isLoading, isFetching } = useUserSoftSkills({
		userId,
		filters,
		top: rowsPerPage,
		skip: page * rowsPerPage,
		order,
		orderBy,
	});

	const showSkeleton = isLoading || isFetching;

	const { mutateUpdateUserSoftSkill, mutateCreateSoftSkill } =
		useSoftSkillsMutations({
			handleClose,
			snapshotId: id,
			userId,
		});

	const handleEditItemClick = useCallback(
		(itemId?: string | number) => () => {
			handleOpen(itemId);
		},
		[]
	);

	const handleAddItemClick = useCallback(() => {
		handleOpen();
	}, []);

	const snapshotsDates = useGetAllSnapshotsDates(data?.items);

	const columns = useSoftSkillsTableColumns(
		filters,
		setFilters,
		handleEditItemClick,
		snapshotsDates,
		data?.items
	);

	const rows = useSoftSkillsTableRows(data?.items, showSkeleton);

	const softSkillsTablePagination = useMemo(
		() =>
			createGridPagination(
				rows.length,
				page,
				setPage,
				rowsPerPage,
				handleOnPerPageChange,
				data?.total
			),
		[rows.length, page, rowsPerPage, data?.total]
	);

	const updateSoftSkills = (values: SeniorityFormsValues) => {
		const userSoftSkills = convertDataForRequest(values);

		return mutateUpdateUserSoftSkill({
			userSoftSkills,
		});
	};

	const createSoftSkills = (values: SeniorityFormsValues) => {
		const userSoftSkills = convertDataForRequest(values);

		return mutateCreateSoftSkill({
			snapshotDate: dayjs(values.date).format(YYYY_MM_DD_DAYJS_DATE_FORMAT),
			userSoftSkills,
		});
	};

	const handleFormSubmit = (values: SeniorityFormsValues) => {
		const submitFunction = id ? updateSoftSkills : createSoftSkills;

		return submitFunction(values);
	};

	const isDataLoading = isLoading || isFetching;

	const isLastSnapshotDateEqualCurrentDate = useMemo(
		() =>
			snapshotsDates &&
			snapshotsDates.length > 0 &&
			areDatesEqual(dayjs(snapshotsDates[0].snapshotDate).toDate(), new Date()),
		[snapshotsDates]
	);

	const isAddButtonDisabled =
		isDataLoading || isLastSnapshotDateEqualCurrentDate || rows.length === 0;

	return (
		<TabTableWrapper>
			<TabHeader
				onAddClick={handleAddItemClick}
				isButtonDisabled={isAddButtonDisabled}
				buttonTitle={HEADER_BUTTON_TITLE}
			/>
			<Loader isLoading={isDataLoading} size="small">
				<SoftSkillsTable
					sortColumns={sortColumns}
					rows={rows}
					paginationProps={softSkillsTablePagination}
					columns={columns}
					snapshotsDates={snapshotsDates}
					handleSortColumnChange={handleSortColumnChange}
				/>
			</Loader>

			{isOpen && userId && (
				<SeniorityForm
					userId={userId}
					editSnapshotId={id}
					handleSubmit={handleFormSubmit}
					snapshotDate={snapshotsDates[0]?.snapshotDate}
					handleClose={handleClose}
				/>
			)}
		</TabTableWrapper>
	);
}
