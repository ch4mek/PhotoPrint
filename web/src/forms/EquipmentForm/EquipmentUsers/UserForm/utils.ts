import { Roles } from '@/constants/roles';
import { getFullName } from '@/helpers/formatName';
import { EquipmentUsersLookupResponse } from '@/queries/equipment/types';

import { isApproverGM, isApproverHD } from './constants';
import { InitUserFilterState } from './types';

export const getAssignmentApproversOptions = (
	allActiveUsers: EquipmentUsersLookupResponse[],
	{ selectedUser }: InitUserFilterState
) =>
	allActiveUsers
		.filter(({ positionDescription }) => positionDescription !== Roles.SE)
		.filter(({ positionDescription, departmentId, resourceGroupId }) => {
			if (!selectedUser) {
				return true;
			}

			if (isApproverHD(positionDescription)) {
				return selectedUser.departmentId === departmentId;
			}

			if (isApproverGM(positionDescription)) {
				return selectedUser.resourceGroupId === resourceGroupId;
			}

			return true;
		})
		.map(({ firstName, lastName, ...restValues }) => ({
			fullName: getFullName(firstName, lastName),
			...restValues,
		}));

export const getUsersOptions = (
	allActiveUsers: EquipmentUsersLookupResponse[],
	assigmentApproverValue?: Omit<
		EquipmentUsersLookupResponse,
		'firstName' | 'lastName'
	>
) =>
	allActiveUsers
		.map(({ id, firstName, lastName, departmentId, resourceGroupId }) => ({
			id,
			fullName: getFullName(firstName, lastName),
			departmentId,
			resourceGroupId,
		}))
		.filter((user) => {
			if (assigmentApproverValue) {
				const { departmentId, resourceGroupId, positionDescription } =
					assigmentApproverValue;

				if (isApproverHD(positionDescription)) {
					return user.departmentId === departmentId;
				}

				if (isApproverGM(positionDescription)) {
					return user.resourceGroupId === resourceGroupId;
				}
			}

			return true;
		});
