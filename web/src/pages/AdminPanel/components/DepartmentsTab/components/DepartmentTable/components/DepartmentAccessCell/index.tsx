import { Tooltip } from '@mms/mms-ui-library/components/Tooltip';
import { RenderCellProps } from 'react-data-grid';

import { DepartmentGridRow, DepartmentGridRowType } from '../../../../types';
import { GROUP_SUBROW_CLASS_NAME } from '../../constants';
import { newCellHeight } from '../../utils/getNewCellHeight';
import {
	DepartmentsCellWrapper,
	DepartmentsFieldsWrapper,
	DepartmentsGeneralFields,
	DepartmentsGeneralFieldWrapper,
	DepartmentsSubFields,
} from '../DepartmentCellRenderer/styles';

import { AccessText } from './styles';

export function DepartmentAccessCell({
	row,
}: RenderCellProps<DepartmentGridRow>) {
	const { access, type, groups, isExpanded, id } = row;
	const isSubRow = type === DepartmentGridRowType.Group;

	const height = newCellHeight(false, groups?.length || 1);

	const firstRowContent = (
		<Tooltip useIfTextCut text={access}>
			<AccessText variant={isSubRow ? 's-400' : 'm-400'}>{access}</AccessText>
		</Tooltip>
	);

	return (
		<DepartmentsCellWrapper data-expanded={isExpanded}>
			<DepartmentsFieldsWrapper>
				<DepartmentsGeneralFields
					className={GROUP_SUBROW_CLASS_NAME}
					data-subrowhoverindex={id}
					newHeight={height}
				>
					<DepartmentsGeneralFieldWrapper>
						{firstRowContent}
					</DepartmentsGeneralFieldWrapper>
				</DepartmentsGeneralFields>
				{groups?.map(({ id: groupId, access: groupAccess }) => (
					<DepartmentsSubFields
						className={GROUP_SUBROW_CLASS_NAME}
						data-subrowhoverindex={`${id}-${groupId}`}
						key={groupId}
						isAccessCell
					>
						<DepartmentsGeneralFieldWrapper>
							<Tooltip useIfTextCut text={groupAccess}>
								<AccessText variant="s-400">{groupAccess}</AccessText>
							</Tooltip>
						</DepartmentsGeneralFieldWrapper>
					</DepartmentsSubFields>
				))}
			</DepartmentsFieldsWrapper>
		</DepartmentsCellWrapper>
	);
}
