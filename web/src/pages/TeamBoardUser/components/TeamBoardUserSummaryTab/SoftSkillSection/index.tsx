import { useLoadingStatus } from '@mms/mms-ui-library/components/ManagedLoader';
import { useCallback, useContext, useMemo, useRef } from 'react';

import { useSoftSkillsMutations } from '../../SoftSkillsTab/hooks/useSoftSkillsMutations';
import { convertDataForRequest } from '../../SoftSkillsTab/utils/convertDataForRequest';
import { SoftSkillsTable } from '../../SoftSkillsTable';
import { SECTION_HEADER } from '../../SoftSkillsTable/constants';
import { PersonalDataContext } from '../../../context/personalDataContext';
import { TeamBoardTabsId } from '../../../types';
import { TeamboardSectionContainer } from '../TeamboardSectionContainer';

import { InfoDataTableWrapper } from '@/components/InfoDataContainer/InfoDataTableWrapper';
import { YYYY_MM_DD_DAYJS_DATE_FORMAT } from '@/constants/date';
import { SeniorityForm } from '@/forms/TeamboardForm/SeniorityForm';
import { SeniorityFormsValues } from '@/forms/TeamboardForm/SeniorityForm/types';
import { areDatesEqual } from '@/helpers/areValuesEqual';
import dayjs from '@/lib/dayjs';
import { useShowModal } from '@/pages/hooks/useShowModal';
import { useGetAssignedUsersSoftSkills } from '@/queries/softSkills/useGetAssignedUsersSoftSkills';
import { TeamboardTableMode } from '@/types/teamboard';

import { mapSoftSkillsLookupToRows } from './utils';

export function SoftSkillSection() {
	const { handleOpen, isOpen, handleClose, id } = useShowModal();

	const { userId, showLoader } = useContext(PersonalDataContext);
	const previousRowsCountRef = useRef<number>(1);

	const {
		data = [],
		isLoading,
		isFetching,
	} = useGetAssignedUsersSoftSkills(userId, undefined);

	useLoadingStatus(SoftSkillSection.name, isLoading || showLoader);

	const { mutateCreateSoftSkill } = useSoftSkillsMutations({
		handleClose,
		snapshotId: id,
		userId,
	});

	const softSkillsTableRows = useMemo(() => {
		if (!data) {
			return [];
		}

		previousRowsCountRef.current = data.length;

		return mapSoftSkillsLookupToRows(data);
	}, [data, isLoading, isFetching, showLoader]);

	const handleAddClick = useCallback(() => {
		handleOpen();
	}, [handleOpen]);

	const createSoftSkills = (values: SeniorityFormsValues) => {
		const userSoftSkills = convertDataForRequest(values);

		return mutateCreateSoftSkill({
			snapshotDate: dayjs(values.date).format(YYYY_MM_DD_DAYJS_DATE_FORMAT),
			userSoftSkills,
		});
	};

	const snapshotDate = data.find(
		(item) => item.snapshotDate !== null
	)?.snapshotDate;

	const isLastSnapshotDateEqualCurrentDate = useMemo(
		() =>
			snapshotDate && areDatesEqual(dayjs(snapshotDate).toDate(), new Date()),
		[snapshotDate]
	);

	const isDisableAddButton =
		isLoading ||
		isLastSnapshotDateEqualCurrentDate ||
		softSkillsTableRows.length === 0;

	return (
		<>
			<TeamboardSectionContainer
				isDisableAddButton={isDisableAddButton}
				handleAddClick={handleAddClick}
				headerTitle={SECTION_HEADER}
				tabId={TeamBoardTabsId.SOFT_SKILLS}
			>
				<InfoDataTableWrapper>
					<SoftSkillsTable
						rows={softSkillsTableRows}
						mode={TeamboardTableMode.Section}
					/>
				</InfoDataTableWrapper>
			</TeamboardSectionContainer>
			{isOpen && (
				<SeniorityForm
					userId={userId}
					editSnapshotId={id}
					handleSubmit={createSoftSkills}
					snapshotDate={snapshotDate || undefined}
					handleClose={handleClose}
				/>
			)}
		</>
	);
}
