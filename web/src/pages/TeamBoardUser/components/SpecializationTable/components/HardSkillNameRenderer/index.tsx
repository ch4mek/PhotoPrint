import { RenderCellProps } from 'react-data-grid';

import { HardSkillsTableCell } from '../HardSkillsTable';
import { SpecializationCell } from '../SpecializationCell';

import {
	SpecializationRow,
	SpecializationRowType,
} from '@/pages/TeamBoardUser/types';

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
