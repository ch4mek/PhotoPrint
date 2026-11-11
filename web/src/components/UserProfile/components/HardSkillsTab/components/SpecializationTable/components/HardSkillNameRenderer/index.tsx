import { RenderCellProps } from 'react-data-grid';

import { SpecializationRow, SpecializationRowType } from '../../../../types';
import { HardSkillsTableCell } from '../HardSkillsTable';
import { SpecializationCell } from '../SpecializationCell';

export function HardSkillNameRenderer(
	props: RenderCellProps<SpecializationRow, unknown>
) {
	const { row } = props;

	const { type } = row;

	if (type === SpecializationRowType.Specialization) {
		return <SpecializationCell {...props} />;
	}

	return <HardSkillsTableCell {...props} />;
}
