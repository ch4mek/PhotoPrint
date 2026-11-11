import { SkillsNames } from '../../constants';
import { SkillListColor } from '../../types';
import { TileList } from '../TileList';

import { useConfiguration } from '@/context/Configuration';

import { DepartmentsListProps } from './types';

export function DepartmentsList({
	selectedDepartmentId,
	handleDepartmentItemClick,
}: DepartmentsListProps) {
	const {
		configurableValues: { departmentTypes },
	} = useConfiguration();

	return (
		<TileList
			selectedIndex={selectedDepartmentId}
			fieldToDisplay="fullName"
			onSelect={handleDepartmentItemClick}
			beforeColor={SkillListColor.Gray}
			items={departmentTypes}
			totalItems={departmentTypes.length}
			title={SkillsNames.departmentsList}
			isPointerCursor
		/>
	);
}
