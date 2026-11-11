import { spaces } from '@mms/mms-ui-library/theme';

import { convertDataToUserCard } from '../../utils/convertDataToUserCard';

import { FormSection } from '@/components/FormSection';
import { UserCard } from '@/components/UserCard';
import { useAuth } from '@/context/AuthContext';

import { participantsTypes } from './constants';
import { RequestParticipantsWrapper } from './styles';
import { RequestParticipantsProps } from './types';

const { created, requestFor, manager } = participantsTypes;

export function RequestParticipants({
	requestFromData,
	managerData,
	createdData,
}: RequestParticipantsProps) {
	const { id: currentUserId } = useAuth();
	const requestFromUser = convertDataToUserCard(requestFromData, currentUserId);
	const managerUser = convertDataToUserCard(managerData, currentUserId);
	const createdFromUser = convertDataToUserCard(createdData, currentUserId);

	return (
		<RequestParticipantsWrapper>
			{requestFromUser && (
				<FormSection title={requestFor} gap={spaces.s}>
					<UserCard
						{...requestFromUser}
						isProfileLinkEnabled={requestFromUser.showProfileLink}
					/>
				</FormSection>
			)}
			{managerUser && (
				<FormSection title={manager} gap={spaces.s}>
					<UserCard
						{...managerUser}
						isProfileLinkEnabled={managerUser.showProfileLink}
					/>
				</FormSection>
			)}
			{createdFromUser && (
				<FormSection title={created} gap={spaces.s}>
					<UserCard
						{...createdFromUser}
						isProfileLinkEnabled={createdFromUser.showProfileLink}
					/>
				</FormSection>
			)}
		</RequestParticipantsWrapper>
	);
}
