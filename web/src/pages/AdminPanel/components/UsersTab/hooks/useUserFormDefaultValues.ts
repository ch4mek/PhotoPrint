import { useMemo } from 'react';

import { UseUserFormDefaultValuesProps } from '../types';

import { isHeadOrGroupManager, Roles } from '@/constants/roles';
import { useAuth } from '@/context/AuthContext';
import { useConfiguration } from '@/context/Configuration';
import { UserFormValues } from '@/forms/types';
import { getRandomColor } from '@/helpers/getRandomColor';

export const useUserFormDefaultValues = ({
	data,
	editId,
	isEditOpen,
	departmentsInfo,
}: UseUserFormDefaultValuesProps) => {
	const { role, departmentId } = useAuth();

	const {
		configurableValues: { departmentTypes, roleTypes, countries },
		persistentValues: { locationTypes },
	} = useConfiguration();

	const defaultValues = useMemo((): UserFormValues => {
		const item = data?.items.find((user) => user.id === editId);

		if (editId && item) {
			const {
				isActive,
				firstName,
				lastName,
				firstNameRu,
				lastNameRu,
				patronymicRu,
				email,
				position,
				color,
				resourceGroupId,
				currentLocation,
			} = item;

			return {
				firstName,
				lastName,
				firstNameRu: firstNameRu ?? '',
				lastNameRu: lastNameRu ?? '',
				patronymicRu: patronymicRu ?? '',
				email,
				group:
					departmentsInfo
						?.find(({ departmentName }) => departmentName === item.departmentId)
						?.groups?.find(({ id: groupId }) => groupId === resourceGroupId) ??
					null,
				position: roleTypes?.find((rol) => rol.id === position) ?? null,
				department:
					departmentTypes?.find(
						(department) => department.id === item.departmentId
					) ?? null,
				currentLocation: currentLocation ?? '',
				location:
					locationTypes.find((location) => location.id === item.location) ??
					null,
				projects: [],
				isActive,
				color,
				registeredLocation:
					countries.find(
						(country) => String(country.id) === item.registeredLocation
					) ?? null,
				registeredLocationId:
					countries.find((country) => country.id === item.registeredLocationId)
						?.id ?? null,
			};
		}

		return {
			firstName: '',
			lastName: '',
			firstNameRu: '',
			lastNameRu: '',
			patronymicRu: '',
			email: '',
			position: isHeadOrGroupManager(role)
				? roleTypes?.find(({ name }) => name === Roles.SE) ?? null
				: null,
			department: isHeadOrGroupManager(role)
				? departmentTypes?.find(
						({ id: configurationDepartmentId }) =>
							configurationDepartmentId === departmentId
				  ) ?? null
				: null,
			group: null,
			currentLocation: '',
			location: null,
			projects: [],
			color: getRandomColor(),
			isActive: true,
			registeredLocation: null,
			registeredLocationId: null,
		};
	}, [
		isEditOpen,
		editId,
		data,
		departmentsInfo,
		departmentTypes,
		roleTypes,
		locationTypes,
		countries,
	]);

	return { defaultValues };
};
