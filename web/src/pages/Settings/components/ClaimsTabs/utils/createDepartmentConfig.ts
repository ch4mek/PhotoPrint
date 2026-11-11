import {
	ADMINISTRATION_DEPARTMENT,
	ADMINISTRATION_DEPARTMENT_WIDTH,
	DEPARTMENT_COLUMN_WIDTH,
} from '../constants';
import { ClaimsRowConfig } from '../types';

import { ComplexConfigurableConfigurationItem } from '@/types/configuration';

export const createDepartmentsObjectConfig = (
	departments: ComplexConfigurableConfigurationItem[]
): Record<string, ClaimsRowConfig> =>
	departments.reduce((acc, { id, shortName }) => {
		acc[shortName] = {
			name: shortName,
			width:
				shortName === ADMINISTRATION_DEPARTMENT
					? ADMINISTRATION_DEPARTMENT_WIDTH
					: DEPARTMENT_COLUMN_WIDTH,
			key: id,
		};

		return acc;
	}, {} as Record<string, ClaimsRowConfig>);
