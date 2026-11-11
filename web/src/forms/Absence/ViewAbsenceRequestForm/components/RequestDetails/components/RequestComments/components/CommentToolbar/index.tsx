import { AttachFileIcon } from '@mms/mms-ui-library/icons/base/AttachFileIcon';
import { FormatBoldIcon } from '@mms/mms-ui-library/icons/editor/FormatBoldIcon';
import { FormatItalicIcon } from '@mms/mms-ui-library/icons/editor/FormatItalicIcon';
import { FormatListBulletedIcon } from '@mms/mms-ui-library/icons/editor/FormatListBulletedIcon';
import { FormatUnderlinedIcon } from '@mms/mms-ui-library/icons/editor/FormatUnderlinedIcon';
import { LinkIcon } from '@mms/mms-ui-library/icons/editor/LinkIcon';
import {
	ELEMENT_UL,
	insertLink,
	MARK_BOLD,
	MARK_ITALIC,
	MARK_UNDERLINE,
} from '@udecode/plate';
import React, { ChangeEvent, useRef } from 'react';
import { BaseEditor, Editor, Node } from 'slate';

import { mapSelectedFilesToAppFiles } from '../../utils/mapUploadedFilesToAppFiles';

import { AttachButton } from '@/components/AttachButton';
import { TOOLBAR_BUTTON_CLASSNAME } from '@/components/PlateToolbars/constants';
import { useToolbarPopUp } from '@/components/PlateToolbars/hooks/useToolbarPopUp';
import { DEFAULT_MAXIMUM_ALLOWED_REQUEST_FILE_SIZE_KB } from '@/constants/allowedFileSizes';
import { IconSize } from '@/constants/iconSizes';
import { useAttachDocument } from '@/hooks/useAttachDocument';

import {
	WithAlwaysVisibleLinkButton,
	WithAlwaysVisibleListButton,
	WithAlwaysVisibleMarkButton,
} from './components/WithAlwaysVisibleMarkButton';
import { MAX_FILES_COUNT, VALID_EXTENSIONS } from './constants';
import { CommentToolbarWrapper } from './styles';
import { CommentToolbarProps } from './types';

function CommentToolbarInner({
	files,
	editor,
	onSetFiles,
}: CommentToolbarProps) {
	const hiddenFileInput = useRef<HTMLInputElement>(null);

	const { handleFileChange } = useAttachDocument(
		VALID_EXTENSIONS,
		DEFAULT_MAXIMUM_ALLOWED_REQUEST_FILE_SIZE_KB
	);

	const handleAttachedFileChange = () => (e: ChangeEvent<HTMLInputElement>) => {
		const selectedFiles = handleFileChange(e, files.length, MAX_FILES_COUNT);

		if (selectedFiles) {
			const newFiles = mapSelectedFilesToAppFiles(selectedFiles, files.length);
			onSetFiles((prevState) => [...prevState, ...newFiles]);
		}

		return selectedFiles;
	};

	const { handleClose } = useToolbarPopUp();

	const handleSubmit = ({ url }: { url: string }) => {
		if (!editor) {
			return;
		}

		const selectedText = editor.selection
			? Node.string({
					children: Editor.fragment(editor as BaseEditor, editor.selection),
			  })
			: url;

		insertLink(editor, { url, text: selectedText });
		handleClose();
	};

	const toolbarConfig = [
		WithAlwaysVisibleMarkButton(MARK_BOLD, <FormatBoldIcon />),
		WithAlwaysVisibleMarkButton(MARK_ITALIC, <FormatItalicIcon />),
		WithAlwaysVisibleMarkButton(MARK_UNDERLINE, <FormatUnderlinedIcon />),
		WithAlwaysVisibleListButton(ELEMENT_UL, <FormatListBulletedIcon />),
		WithAlwaysVisibleLinkButton(<LinkIcon />, handleSubmit),
	];

	return (
		<CommentToolbarWrapper>
			{toolbarConfig.map((ButtonComponent, idx) => (
				<ButtonComponent
					key={idx}
					editor={editor}
					className={TOOLBAR_BUTTON_CLASSNAME}
				/>
			))}

			<AttachButton
				disabled={(files.length || 0) >= MAX_FILES_COUNT}
				onAttachedFileChange={handleAttachedFileChange}
				validExtensions={VALID_EXTENSIONS}
				ref={hiddenFileInput}
				icon={<AttachFileIcon width={IconSize.MS} />}
				isAttachMultiple
			/>
		</CommentToolbarWrapper>
	);
}

export const CommentToolbar = React.memo(CommentToolbarInner);
