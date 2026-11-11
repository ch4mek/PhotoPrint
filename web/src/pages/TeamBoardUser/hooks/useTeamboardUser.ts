import { useCallback, useEffect, useMemo } from 'react';

import { TeamboardUserAccessLevel, TeamboardUserProfileKind } from '../types';
import { getProfileKind } from '../utils/getProfileKind';
import { isCurrentUserManagerOfPickedUser } from '../utils/isCurrentUserManagerOfPickedUser';

import { SOMETHING_WENT_WRONG } from '@/constants/errors';
import { NotificationType, USER_NOT_FOUND } from '@/constants/notifications';
import { useAuth } from '@/context/AuthContext';
import { useToast } from '@/context/Toast';
import { checkIsHasClaims } from '@/helpers/checkIsHasClaims';
import { createToast } from '@/helpers/createToast';
import { usePersonalData } from '@/queries/personalData/usePersonalData';
import { UserClaims } from '@/types/claims';

export const useTeamboardUser = (
	id: number | string | undefined,
	onClose: VoidFunction,
	onRefetch?: VoidFunction
) => {
	const {
		claims,
		role,
		departmentId: currentUserDepartmentId,
		resourceGroupId: currentUserGroupId,
	} = useAuth();

	const toast = useToast();

	const {
		data,
		isFetching,
		refetch: refetchPersonalData,
		isLoading,
	} = usePersonalData(id, {
		onError: (error) => {
			toast?.open(
				createToast(
					NotificationType.ERROR,
					error.response?.data.Message || SOMETHING_WENT_WRONG
				)
			);
			onClose();
		},
	});

	const refetch = useCallback(() => {
		refetchPersonalData();
		onRefetch?.();
	}, [refetchPersonalData, onRefetch]);

	const profileKind = useMemo(
		() => getProfileKind(data?.position),
		[data?.position]
	);

	const accessLevel = useMemo<TeamboardUserAccessLevel>(() => {
		if (!data) {
			return TeamboardUserAccessLevel.None;
		}

		const isUserManagerOfPickerUser = isCurrentUserManagerOfPickedUser({
			currentUserRole: role,
			pickedUserRole: data.position,
			currentUserDepartmentId,
			pickedUserDepartmentId: data.departmentId,
			currentUserGroupId,
			pickedUserGroupId: data.resourceGroupId,
		});

		if (isUserManagerOfPickerUser) {
			return TeamboardUserAccessLevel.Full;
		}

		if (
			checkIsHasClaims([UserClaims.ManageAllInterns], claims) &&
			profileKind === TeamboardUserProfileKind.Intern
		) {
			return TeamboardUserAccessLevel.Full;
		}

		if (checkIsHasClaims([UserClaims.ManageHRModule], claims)) {
			return TeamboardUserAccessLevel.LimitedHR;
		}

		if (checkIsHasClaims([UserClaims.ManageProjectCoordinator], claims)) {
			return TeamboardUserAccessLevel.LimitedCoordinator;
		}

		if (checkIsHasClaims([UserClaims.ManageAccountant], claims)) {
			return TeamboardUserAccessLevel.LimitedAccountant;
		}

		return TeamboardUserAccessLevel.None;
	}, [
		data,
		role,
		currentUserDepartmentId,
		currentUserGroupId,
		claims,
		profileKind,
	]);

	useEffect(() => {
		if (!data || accessLevel !== TeamboardUserAccessLevel.None) {
			return;
		}

		toast?.open(
			createToast(NotificationType.ERROR, USER_NOT_FOUND, USER_NOT_FOUND)
		);
		onClose();
	}, [data, accessLevel, toast, onClose]);

	return {
		data,
		isFetching,
		isLoading,
		accessLevel,
		profileKind,
		refetch,
	};
};
