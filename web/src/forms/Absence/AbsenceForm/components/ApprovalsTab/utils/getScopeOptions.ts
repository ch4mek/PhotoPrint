import { roleOptions } from '../components/ApprovelsFieldsGroup/constants';
import { RoleScopes } from '../constants';

import { AcceptPolicesItem, AllRolesOptions } from '@/types/absences';

import { checkIsAnyUserRole } from './checkIsAnyUserRole';

export const getScopeOptions = (
	fieldsValues: AcceptPolicesItem[],
	isSubstitute: boolean = false,
	excludeScope?: string | null
) => {
	const fieldKey = isSubstitute ? RoleScopes.SubstituteScope : RoleScopes.Scope;

	return roleOptions.filter(({ scope }) => {
		if (checkIsAnyUserRole(scope)) {
			return true;
		}

		if (isSubstitute && scope === AllRolesOptions.PM) {
			return false;
		}

		if (!isSubstitute) {
			const isAlreadyUsed = fieldsValues.some(
				(item) => item?.[fieldKey]?.scope === scope
			);

			if (isAlreadyUsed) {
				return false;
			}
		}

		if (excludeScope && scope === excludeScope) {
			return false;
		}

		return true;
	});
};
