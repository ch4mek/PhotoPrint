import { useTheme } from '@emotion/react';
import { Spinner } from '@mms/mms-ui-library/components/Spinner';
import { Typography } from '@mms/mms-ui-library/components/Typography';
import { memo, useDeferredValue, useEffect, useRef } from 'react';

import { UserCard } from '@/components/UserCard';
import { isNotChiefRole } from '@/constants/roles';

import {
	AssignUserCard,
	AssignUserCardList,
	FetchNextPageLoader,
} from './styles';
import { AssignsListProps } from './types';
import { getAssignmentText } from './utils';

function UsersListInner({
	items,
	hasMoreItems,
	onNextPage: handleNextPage,
}: AssignsListProps) {
	const intersectionObserver = useRef<IntersectionObserver>(null);
	const assignUsers = useDeferredValue(items);
	const loaderRef = useRef<HTMLDivElement>(null);
	const theme = useTheme();

	useEffect(() => {
		if (!loaderRef.current || !hasMoreItems) {
			return;
		}

		intersectionObserver.current = new IntersectionObserver((entries) => {
			const observedTarget = entries[0];

			if (observedTarget.isIntersecting && hasMoreItems) {
				handleNextPage();
			}
		});

		intersectionObserver.current.observe(loaderRef.current);

		return () => {
			intersectionObserver.current?.disconnect();
		};
	}, [hasMoreItems, handleNextPage]);

	return (
		<AssignUserCardList>
			{assignUsers.map(
				({
					userFullName,
					userFirstName,
					userLastName,
					avatarFileId,
					department,
					position,
					userId,
					dateFrom,
					dateTo,
				}) => (
					<AssignUserCard key={userId}>
						<UserCard
							avatarFieldId={avatarFileId}
							firstName={userFirstName}
							lastName={userLastName}
							fullName={userFullName}
							department={isNotChiefRole(position) ? department : position}
						/>
						<Typography variant="s-400" color={theme.palette['text-gray']}>
							{getAssignmentText(dateFrom, dateTo)}
						</Typography>
					</AssignUserCard>
				)
			)}
			{hasMoreItems && (
				<FetchNextPageLoader ref={loaderRef}>
					<Spinner size="small" />
				</FetchNextPageLoader>
			)}
		</AssignUserCardList>
	);
}

export const UsersList = memo(UsersListInner);
