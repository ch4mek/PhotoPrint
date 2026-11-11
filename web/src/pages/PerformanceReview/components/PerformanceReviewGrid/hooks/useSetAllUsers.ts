import { useEffect } from 'react';

import { SOMETHING_WENT_WRONG } from '@/constants/errors';
import { NotificationType } from '@/constants/notifications';
import { useToast } from '@/context/index';
import { createToast } from '@/helpers/createToast';
import { getFullName } from '@/helpers/utils';
import { useTeamboardUsersLookup } from '@/queries/teamboard/useTeamboardUsersLookup';
import { createSelector } from '@/stores/helpers';
import { usePerformanceReviewStore } from '@/stores/performanceReview/store';
import { PerformanceReviewUserData } from '@/types/performanceReview';

export function useSetAllUsers() {
	const toast = useToast();
	const { data, error } = useTeamboardUsersLookup();

	const { setUsers } = usePerformanceReviewStore(createSelector('setUsers'));

	useEffect(() => {
		if (error) {
			toast.open(createToast(NotificationType.ERROR, SOMETHING_WENT_WRONG));
		}
	}, [error]);

	useEffect(() => {
		if (data) {
			const usersData: PerformanceReviewUserData[] = [];

			data.forEach(({ firstName, lastName, id, firstNameRu, lastNameRu }) => {
				const fullName = getFullName(
					firstName,
					lastName,
					firstNameRu,
					lastNameRu
				);

				usersData.push({
					id,
					fullName,
				});
			});

			setUsers(usersData);
		}
	}, [data]);
}
