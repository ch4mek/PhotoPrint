import { useTheme } from '@emotion/react';
import { PickerButtonOption } from '@mms/mms-ui-library/components/PickerButton';
import { Typography } from '@mms/mms-ui-library/components/Typography';
import { AttachFileIcon } from '@mms/mms-ui-library/icons/base/AttachFileIcon';
import { useMemo, useState } from 'react';

import {
	AttachFileButtonWrapper,
	AttachPickerButton,
	HiddenInput,
} from '@/components/AttachButton/styles';
import { AttachButtonProps } from '@/components/AttachButton/types';
import { formatAllowedFileTypesToAccept } from '@/helpers/formatAllowedFileTypes';
import { isRefObject } from '@/helpers/isRefObject';

export function AttachButton({
	onAttachedFileChange,
	validExtensions,
	title,
	fileTypes = [],
	ref,
	icon,
	isAttachMultiple = false,
	disabled = false,
}: AttachButtonProps) {
	const theme = useTheme();
	const [selectedFileType, setSelectedFileType] = useState<string>();

	const openFileSelection = () => {
		if (ref && isRefObject(ref)) {
			ref.current?.click();
		}
	};

	const handleFileTypeOptionsClick = (fileType: string) => () => {
		setSelectedFileType(fileType);
		openFileSelection();
	};
	const fileTypesOptions = useMemo<Array<PickerButtonOption>>(
		() =>
			fileTypes.map((typeName) => ({
				value: typeName,
				onClick: handleFileTypeOptionsClick(typeName),
			})),
		[fileTypes]
	);

	const attachButtonContent = (
		<>
			{icon ?? <AttachFileIcon color={theme.palette['graphics-accent']} />}
			{title && (
				<Typography variant="m-400" color={theme.palette['text-accent']}>
					{title}
				</Typography>
			)}
		</>
	);

	if (fileTypes.length) {
		return (
			<>
				<AttachPickerButton
					icon={
						<AttachFileButtonWrapper>
							{attachButtonContent}
						</AttachFileButtonWrapper>
					}
					options={fileTypesOptions}
					popoverPosition="top-start"
				/>
				<HiddenInput
					type="file"
					hidden
					ref={ref}
					onChange={onAttachedFileChange(selectedFileType)}
					accept={formatAllowedFileTypesToAccept(validExtensions)}
					multiple={isAttachMultiple}
				/>
			</>
		);
	}

	return (
		<AttachFileButtonWrapper onClick={openFileSelection} disabled={disabled}>
			{attachButtonContent}
			<HiddenInput
				type="file"
				hidden
				ref={ref}
				onChange={onAttachedFileChange(selectedFileType)}
				accept={formatAllowedFileTypesToAccept(validExtensions)}
				multiple={isAttachMultiple}
			/>
		</AttachFileButtonWrapper>
	);
}
