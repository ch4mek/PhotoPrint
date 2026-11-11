import { chiefRoles, Roles } from '@/constants/roles';
import type { ComplexConfigurableConfigurationItem } from '@/types/configuration';

export const getDepartmentTitle = (
	positionTitle: string,
	department: string | ComplexConfigurableConfigurationItem | null
) => {
	if (chiefRoles.includes(positionTitle as Roles)) {
		return null;
	}

	if (typeof department === 'string') {
		return department;
	}

	if (department && department.shortName) {
		return department.shortName;
	}

	return null;
};
