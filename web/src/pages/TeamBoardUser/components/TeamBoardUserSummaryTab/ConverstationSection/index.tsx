import {
	LoaderProvider,
	useLoadingStatus,
} from '@mms/mms-ui-library/components/ManagedLoader';
import { useContext, useEffect, useState } from 'react';

import { ConversationTable } from '../../ConversationTable';
import { PersonalDataContext } from '../../../context/personalDataContext';
import { useConversationFormState } from '../../../hooks/useConversationFormState';
import { ConversationRow } from '../../../types';
import {
	convertConversationResponseToRows,
	getConversationLoaderRows,
} from '../../../utils';
import { TeamboardSectionContainer } from '../TeamboardSectionContainer';

import { InfoDataTableWrapper } from '@/components/InfoDataContainer/InfoDataTableWrapper';
import { ConversationForm } from '@/forms/TeamboardForm/Conversation';
import { useUserConversations } from '@/queries/teamboard/useUserConversations';
import { TeamboardTableMode } from '@/types/teamboard';

import {
	CONVERSATIONS_DATA_CONTAINER_PROPS,
	sectionQueryParams,
} from './constants';

export function ConversationsSection() {
	const [rows, setRows] = useState<Array<ConversationRow>>([]);
	const { userId } = useContext(PersonalDataContext);

	const { data, isLoading } = useUserConversations({
		userId,
		...sectionQueryParams,
	});

	useLoadingStatus(ConversationsSection.name, isLoading);

	const {
		id,
		handleClose,
		handleFormSubmit,
		isOpen,
		handleConservationEditClick,
		handleAddClick,
	} = useConversationFormState();

	useEffect(() => {
		if (isLoading) {
			setRows(getConversationLoaderRows());

			return;
		}

		if (data?.items) {
			setRows(
				convertConversationResponseToRows(
					data.items,
					handleConservationEditClick
				)
			);
		}
	}, [data, isLoading]);

	return (
		<>
			<TeamboardSectionContainer
				{...CONVERSATIONS_DATA_CONTAINER_PROPS}
				handleAddClick={handleAddClick}
			>
				<InfoDataTableWrapper>
					<ConversationTable
						mode={TeamboardTableMode.Section}
						rows={rows}
						setRows={setRows}
					/>
				</InfoDataTableWrapper>
			</TeamboardSectionContainer>
			{isOpen && (
				<LoaderProvider>
					<ConversationForm
						handleClose={handleClose}
						onSubmit={handleFormSubmit}
						id={id}
					/>
				</LoaderProvider>
			)}
		</>
	);
}
