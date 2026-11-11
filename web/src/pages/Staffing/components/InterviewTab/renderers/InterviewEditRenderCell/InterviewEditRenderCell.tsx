import {
	PickerButton,
	PickerButtonOption,
} from '@mms/mms-ui-library/components/PickerButton';
import { AddIcon } from '@mms/mms-ui-library/icons/base/AddIcon';
import { BasketIcon } from '@mms/mms-ui-library/icons/base/BasketIcon';
import { EditIcon } from '@mms/mms-ui-library/icons/base/EditIcon';
import { OptionDotsIcon } from '@mms/mms-ui-library/icons/base/OptionDotsIcon';
import { useMemo } from 'react';
import { RenderCellProps } from 'react-data-grid';

import { InterviewGridRow } from '../../types';

import { controlsText } from '@/constants/controlsTexts';

export function InterviewEditRenderCell({
	row,
}: RenderCellProps<InterviewGridRow>) {
	const menuOptions = useMemo<Array<PickerButtonOption>>(() => {
		if (row.isMainInterview) {
			return [
				{
					addon: <AddIcon />,
					onClick: row.handleAddStage ? row.handleAddStage(row.id) : () => {},
					value: 'Add stage',
				},
				{
					addon: <EditIcon />,
					onClick: row.handleEdit ? row.handleEdit(row.id) : () => {},
					value: controlsText.editText,
				},
				{
					addon: <BasketIcon />,
					onClick: row.handleDelete ? row.handleDelete(row.id) : () => {},
					value: controlsText.deleteText,
				},
			];
		}

		return [
			{
				addon: <EditIcon />,
				onClick: row.handleEdit ? row.handleEdit(row.id) : () => {},
				value: controlsText.editText,
			},
			{
				addon: <BasketIcon />,
				onClick: row.handleDelete ? row.handleDelete(row.id) : () => {},
				value: controlsText.deleteText,
			},
		];
	}, [row]);

	return (
		<PickerButton
			icon={<OptionDotsIcon />}
			options={menuOptions}
			item={row}
			popoverPosition="left-start"
		/>
	);
}
