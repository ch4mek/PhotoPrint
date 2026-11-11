import { useCallback, useContext, useMemo, useState } from 'react';

import { PersonalDataContext } from '../../context/personalDataContext';
import { SpecializationTable } from '../SpecializationTable';

import { Loader } from '@/components/Loader';
import { TabTableWrapper } from '@/components/TabWrapper';
import { UserSpecializationForm } from '@/forms/TeamboardForm/UserSpecialization';
import { UserSpecializationsForm } from '@/forms/TeamboardForm/UserSpecializations';
import { createGridPagination } from '@/helpers/createGridPagination';
import { useShowModal } from '@/pages/hooks/useShowModal';
import { useTableState } from '@/pages/hooks/useTableState';
import { DeleteSpecializationPopup } from '@/pages/TeamBoardUser/components/HardSkillsTab/components/DeleteSpecializationPopup';
import { useSpecializationDetails } from '@/pages/TeamBoardUser/components/HardSkillsTab/hooks/useSpecializationDetails';
import { useTabSpecializationTableColumns } from '@/pages/TeamBoardUser/components/SpecializationTable/hooks/useTabSpecializationTableColumns';
import { useTabSpecializationTableRows } from '@/pages/TeamBoardUser/components/SpecializationTable/hooks/useTabSpecializationTableRows';
import { useGetSpecializationsSnapshots } from '@/queries/teamboard/useGetSpecializationsSnapshots';
import {
	SpecializationToDelete,
	SpecializationToPrimary,
} from '@/types/teamboard';

import { SetPrimarySpecializationPopup } from './components/SetPrimarySpecializationPopup';
import { SpecializationTabHeader } from './components/SpecializationTabHeader';
import { useHardSkillsMutations } from './hooks/useHardSkillsMutations';
import { useMutateSpecializations } from './hooks/useMutateHardSkills';
import { useSpecializationFilters } from './hooks/useSpecializationFilters';

export function HardSkillsTab() {
	const { filters, setFilters } = useSpecializationFilters();
	const { userId } = useContext(PersonalDataContext);

	const { rowsPerPage, handleOnPerPageChange, page, setPage } = useTableState({
		resetPageDeps: [filters],
	});

	const { data, isFetching, isLoading } = useGetSpecializationsSnapshots(
		userId,
		page,
		rowsPerPage,
		filters.specializationName || undefined
	);

	const showSkeleton = isLoading || isFetching;

	const {
		handleOpen: handleOpenSpecializationFormOpen,
		isOpen: isSpecializationFormOpen,
		handleClose,
	} = useShowModal<string>();
	const {
		id: entryEditFormSnapshotId,
		handleOpen: handleEntryEditFormOpen,
		isOpen: isEntryEditFormOpen,
		handleClose: handleEntryEditFormClose,
	} = useShowModal<number>();
	const {
		id: entryAddFormSnapshotId,
		handleOpen: handleEntryAddFormOpen,
		isOpen: isEntryAddFormOpen,
		handleClose: handleEntryAddFormClose,
	} = useShowModal<number>();

	const [specializationToDelete, setSpecializationToDelete] =
		useState<SpecializationToDelete | null>(null);

	const [specializationToPrimary, setSpecializationToPrimary] =
		useState<SpecializationToPrimary | null>(null);

	const { deleteFunction, updatePrimaryFunction } = useMutateSpecializations();

	const handleOpenDeleteDialog = (specialization: SpecializationToDelete) => {
		setSpecializationToDelete(specialization);
	};

	const handleCloseDeleteDialog = () => {
		setSpecializationToDelete(null);
	};

	const handleCloseSetPrimaryDialog = () => {
		setSpecializationToPrimary(null);
	};

	const handleDeleteSpecialization = useCallback(() => {
		if (specializationToDelete?.id) {
			deleteFunction(specializationToDelete?.id);
		}
		handleCloseDeleteDialog();
	}, [specializationToDelete]);

	const handleAddClick = useCallback(() => {
		handleOpenSpecializationFormOpen();
	}, []);

	const handleEntryEditClick = useCallback((snapshotIdToEdit: number) => {
		handleEntryEditFormOpen(snapshotIdToEdit);
	}, []);

	const handlePrimarySpecialization = (specializationId: number) => {
		if (userId) {
			updatePrimaryFunction({
				userId: +userId as number,
				newSpecializationTemplateId: specializationId,
			});
		}
	};

	const onSpecializationSetPrimaryClick = useCallback(
		(specialization: SpecializationToPrimary) => {
			setSpecializationToPrimary(specialization);
		},
		[]
	);

	const handleSetSpecializationPrimary = useCallback(() => {
		if (specializationToPrimary?.specializationTemplateId) {
			handlePrimarySpecialization(
				specializationToPrimary?.specializationTemplateId
			);
		}
		handleCloseSetPrimaryDialog();
	}, [specializationToPrimary]);

	const { getSpecializationsFormSubmitFunction } =
		useHardSkillsMutations(userId);

	const handleColumnEditButtonClick = useCallback(
		(snapshotEditId: number) => () => {
			handleEntryEditClick(snapshotEditId);
		},
		[handleEntryEditClick]
	);

	const {
		allDates,
		primarySpecializationId,
		isLastSnapshotDateEqualCurrentDate,
		isSpecializationsEmpty,
		lastSnapshotDate,
	} = useSpecializationDetails(data?.items);

	const handleEntryAddClick = useCallback(() => {
		handleEntryAddFormOpen(allDates[0].relatedSnapshotId);
	}, [allDates]);

	const rows = useTabSpecializationTableRows(data?.items, showSkeleton);

	const columns = useTabSpecializationTableColumns(
		data?.items,
		allDates,
		filters,
		setFilters,
		handleColumnEditButtonClick
	);

	const gridPagination = useMemo(
		() =>
			createGridPagination(
				data?.items.length || 0,
				page,
				setPage,
				rowsPerPage,
				handleOnPerPageChange,
				data?.total
			),
		[data?.items.length, page, rowsPerPage, data?.total]
	);

	const isDataLoading = isLoading || isFetching;

	return (
		<>
			<TabTableWrapper>
				<SpecializationTabHeader
					isEntryButtonDisabled={
						isSpecializationsEmpty || isLastSnapshotDateEqualCurrentDate
					}
					onSpecializationAddClick={handleAddClick}
					onEntryAddClick={handleEntryAddClick}
					isButtonDisabled={isDataLoading}
				/>
				<Loader isLoading={isDataLoading} size="small">
					<SpecializationTable
						allDates={allDates}
						onSpecializationSetPrimaryClick={onSpecializationSetPrimaryClick}
						onSpecializationDeleteClick={handleOpenDeleteDialog}
						rows={rows}
						columns={columns}
						gridPagination={gridPagination}
					/>
				</Loader>
				{specializationToPrimary && (
					<SetPrimarySpecializationPopup
						nameOfSpecializationPrimary={specializationToPrimary.name}
						handleSetPrimarySpecialization={handleSetSpecializationPrimary}
						handleCloseSetPrimaryDialog={handleCloseSetPrimaryDialog}
					/>
				)}
				{isSpecializationFormOpen && (
					<UserSpecializationForm
						handleClose={handleClose}
						userId={userId}
						primarySpecializationId={primarySpecializationId}
					/>
				)}
				{isEntryEditFormOpen && (
					<UserSpecializationsForm
						isEditMode
						userId={userId}
						handleClose={handleEntryEditFormClose}
						snapshotId={entryEditFormSnapshotId}
						onSubmit={getSpecializationsFormSubmitFunction(true)}
						minDate={lastSnapshotDate}
					/>
				)}
				{isEntryAddFormOpen && (
					<UserSpecializationsForm
						isEditMode={false}
						userId={userId}
						handleClose={handleEntryAddFormClose}
						snapshotId={entryAddFormSnapshotId}
						onSubmit={getSpecializationsFormSubmitFunction(false)}
						minDate={lastSnapshotDate}
					/>
				)}
			</TabTableWrapper>
			{specializationToDelete && (
				<DeleteSpecializationPopup
					handleCloseDeleteDialog={handleCloseDeleteDialog}
					handleDeleteSpecialization={handleDeleteSpecialization}
					nameOfSpecializationToDelete={specializationToDelete.name}
				/>
			)}
		</>
	);
}
