import {
	PickerButton,
	PickerButtonOption,
} from '@mms/mms-ui-library/components/PickerButton';
import { EditIcon } from '@mms/mms-ui-library/icons/base/EditIcon';
import { OptionDotsIcon } from '@mms/mms-ui-library/icons/base/OptionDotsIcon';
import { DeleteIcon } from '@udecode/plate';
import { useMemo } from 'react';
import { RenderCellProps } from 'react-data-grid';

import { MicrofrontendsEditCellOptionValues } from '../../constants';
import { MicrofrontendsGridRow } from '../../types';

export function ActionsCell({ row }: RenderCellProps<MicrofrontendsGridRow>) {
	const menuOptions = useMemo<Array<PickerButtonOption>>(
		() => [
			{
				addon: <EditIcon />,
				onClick: row.handleEdit(row.name),
				value: MicrofrontendsEditCellOptionValues.edit,
			},
			{
				addon: <DeleteIcon />,
				onClick: row.handleDelete(row.name),
				value: MicrofrontendsEditCellOptionValues.delete,
			},
		],
		[row]
	);

	return (
		<PickerButton
			icon={<OptionDotsIcon />}
			options={menuOptions}
			item={row}
			popoverPosition="left-start"
		/>
	);
}
