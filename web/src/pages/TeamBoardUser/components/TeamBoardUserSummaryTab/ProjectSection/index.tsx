import { useLoadingStatus } from '@mms/mms-ui-library/components/ManagedLoader';
import { useContext, useMemo } from 'react';

import { ProjectsTable } from '../../ProjectsTable';
import { PersonalDataContext } from '../../../context/personalDataContext';
import { useUserProjectAssignmentFormState } from '../../../hooks/useUserProjectAssingmentFormState';
import { TeamBoardTabsId } from '../../../types';
import { TeamboardSectionContainer } from '../TeamboardSectionContainer';

import { InfoDataTableWrapper } from '@/components/InfoDataContainer/InfoDataTableWrapper';
import { TeamboardQueriesKeys } from '@/constants/queriesKeys';
import { TeamboardUserProjectsForm } from '@/forms/TeamboardForm/UserProject';
import { useUserProjectsLookup } from '@/queries/useUserProjectsLookup';
import { TeamboardTableMode } from '@/types/teamboard';

import {
	convertUserProjectLookupToRows,
	getProjectTableLoaderRows,
} from './utils';

export function ProjectSection() {
	const {
		userId,
		data: { fullName },
	} = useContext(PersonalDataContext);

	const user = useMemo(
		() => ({
			id: userId!,
			fullName,
		}),
		[userId, fullName]
	);

	const { data, isFetching, isLoading } = useUserProjectsLookup(userId);

	useLoadingStatus(ProjectSection.name, isFetching || isLoading);

	const {
		id,
		isOpen,
		handleClose,
		handleProjectEditClick,
		handleFormSubmit,
		handleAddClick,
		showLoader,
	} = useUserProjectAssignmentFormState([
		TeamboardQueriesKeys.userAssignmentsLookup,
		`${userId}`,
	]);

	const projectTableRows = useMemo(() => {
		if (isFetching) {
			return getProjectTableLoaderRows(1);
		}

		if (data) {
			return convertUserProjectLookupToRows(data, handleProjectEditClick);
		}

		return [];
	}, [data, isFetching]);

	return (
		<>
			<TeamboardSectionContainer
				tabId={TeamBoardTabsId.PROJECTS}
				headerTitle="Projects"
				handleAddClick={handleAddClick}
			>
				<InfoDataTableWrapper>
					<ProjectsTable
						rows={projectTableRows}
						mode={TeamboardTableMode.Section}
						isLoading={isLoading}
						isColumnsSortable={false}
					/>
				</InfoDataTableWrapper>
			</TeamboardSectionContainer>
			{isOpen && userId && (
				<TeamboardUserProjectsForm
					handleClose={handleClose}
					onSubmit={handleFormSubmit}
					projectAssignId={id}
					showLoader={showLoader || isFetching}
					user={user}
				/>
			)}
		</>
	);
}
