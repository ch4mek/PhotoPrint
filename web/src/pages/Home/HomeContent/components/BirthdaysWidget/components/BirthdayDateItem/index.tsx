import { Typography } from '@mms/mms-ui-library/components/Typography';

import { UserCard } from '@/components/UserCard';

import {
	Container,
	DateWrapper,
	UserCardsContainer,
	USER_CARD_CLASS_NAME,
} from './styles';
import { BirthdayDateItemProps } from './types';

export function BirthdayDateItem({
	users,
	day,
	monthName,
}: BirthdayDateItemProps) {
	const dateString = `${day} ${monthName.slice(0, 3)}`;

	return (
		<Container>
			<DateWrapper>
				<Typography variant="s-400">{dateString}</Typography>
			</DateWrapper>
			<UserCardsContainer>
				{users.map(({ department, lastName, firstName, avatarFileId }) => (
					<UserCard
						key={`${department}-${lastName}-${firstName}`}
						{...{
							firstName,
							lastName,
							avatarFieldId: avatarFileId ?? null,
							department,
						}}
						className={USER_CARD_CLASS_NAME}
					/>
				))}
			</UserCardsContainer>
		</Container>
	);
}
