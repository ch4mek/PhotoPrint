import { useLoadingStatus } from '@mms/mms-ui-library/components/ManagedLoader';
import { useCallback, useContext, useMemo, useRef } from 'react';

import { getLoaderRows } from '../../HardSkillsTab/utils';
import { HardSkillsTable } from '../../HardSkillsTable';
import { PersonalDataContext } from '../../../context/personalDataContext';
import { TeamBoardTabsId } from '../../../types';
import { TeamboardSectionContainer } from '../TeamboardSectionContainer';

import { InfoDataTableWrapper } from '@/components/InfoDataContainer/InfoDataTableWrapper';
import { UserSpecializationForm } from '@/forms/TeamboardForm/UserSpecialization';
import { useShowModal } from '@/pages/hooks/useShowModal';
import { useUserSpecializationLookup } from '@/queries/useUserHardSkillsLookup';
import { TeamboardTableMode } from '@/types/teamboard';

import { mapHardSkillsLookupToRows } from './utils';

export function HardSkillSection() {
	const { handleOpen, isOpen, handleClose } = useShowModal();

	const { userId, showLoader } = useContext(PersonalDataContext);
	const previousRowsCountRef = useRef<number>(1);

	const { data, isLoading, isFetching } = useUserSpecializationLookup(
		userId,
		undefined
	);

	useLoadingStatus(
		HardSkillSection.name,
		isLoading || isFetching || showLoader
	);

	const handleItemEditClick = useCallback(
		(itemId?: number | string) => () => {
			handleOpen(itemId);
		},
		[handleOpen]
	);

	const hardSkillsTableRows = useMemo(() => {
		if (isLoading || isFetching || showLoader) {
			return getLoaderRows();
		}

		if (!data) {
			return [];
		}

		previousRowsCountRef.current = data.length;

		return mapHardSkillsLookupToRows(data, handleItemEditClick);
	}, [data, isLoading, isFetching, showLoader]);

	const handleAddClick = useCallback(() => {
		handleOpen();
	}, [handleOpen]);

	return (
		<>
			<TeamboardSectionContainer
				handleAddClick={handleAddClick}
				headerTitle="Hard skills"
				tabId={TeamBoardTabsId.HARD_SKILLS}
			>
				<InfoDataTableWrapper>
					<HardSkillsTable
						rows={hardSkillsTableRows}
						mode={TeamboardTableMode.Section}
					/>
				</InfoDataTableWrapper>
			</TeamboardSectionContainer>
			{isOpen && (
				<UserSpecializationForm handleClose={handleClose} userId={userId} />
			)}
		</>
	);
}
