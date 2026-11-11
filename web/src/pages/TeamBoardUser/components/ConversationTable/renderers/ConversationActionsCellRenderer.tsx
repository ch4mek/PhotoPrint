import {
	PickerButton,
	PickerButtonOption,
} from '@mms/mms-ui-library/components/PickerButton';
import { EditIcon } from '@mms/mms-ui-library/icons/base/EditIcon';
import { OptionDotsIcon } from '@mms/mms-ui-library/icons/base/OptionDotsIcon';
import { ViewIcon } from '@mms/mms-ui-library/icons/base/ViewIcon';
import { Skeleton } from '@mui/material';
import { useMemo, useRef } from 'react';
import { RenderCellProps } from 'react-data-grid';

import { controlsText } from '@/constants/controlsTexts';
import { useAddScrollListener } from '@/hooks/useAddScrollListener';
import { ConversationRow } from '@/pages/TeamBoardUser/types';

export function ConversationActionsCellRenderer({
	row,
}: RenderCellProps<ConversationRow>) {
	const { id, handleEditClick, handleViewClick, showSkeleton } = row;

	const pickerButtonRef = useRef<HTMLButtonElement>(null);
	const [isOpenPopover, setIsOpenPopover] =
		useAddScrollListener(pickerButtonRef);

	const menuOptions = useMemo<Array<PickerButtonOption>>(() => {
		const options: Array<PickerButtonOption> = [];

		if (handleViewClick) {
			options.push({
				addon: <ViewIcon />,
				onClick: handleViewClick(id),
				value: controlsText.viewText,
			});
		}

		if (handleEditClick) {
			options.push({
				addon: <EditIcon />,
				onClick: handleEditClick(id),
				value: controlsText.editText,
			});
		}

		return options;
	}, [handleEditClick, handleViewClick, id]);

	if (showSkeleton) {
		return <Skeleton animation="wave" width="100%" />;
	}

	if (!menuOptions.length) {
		return null;
	}

	return (
		<PickerButton
			icon={<OptionDotsIcon />}
			options={menuOptions}
			item={row}
			popoverPosition="left-start"
			isOpenPopover={isOpenPopover}
			setIsOpenPopover={setIsOpenPopover}
			ref={pickerButtonRef}
		/>
	);
}
