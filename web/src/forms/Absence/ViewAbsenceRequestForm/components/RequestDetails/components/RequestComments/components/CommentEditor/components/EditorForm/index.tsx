import { useTheme } from '@emotion/react';
import { Plate } from '@udecode/plate';
import { useEffect, useMemo } from 'react';

import { CommentToolbar } from '../../../CommentToolbar';
import { PlateModes } from '../../constants';

import { initialEditorData } from '@/constants/plateEditor';
import { useAbsenceRequestChanges } from '@/forms/Absence/ViewAbsenceRequestForm/context/AbsenceRequestChangesContext';
import { isEditorEmpty } from '@/helpers/plain';
import { getCustomPluginsWithThemeMode } from '@/plugins/PlateEditorPlugins';
import { CONFIG } from '@/plugins/PlateEditorPlugins/config';

import { EditModeContentSync } from './components/EditModeContentSync/EditModeContentSync';
import { EditorReadyNotifier } from './components/EditorReadyNotifier/EditorReadyNotifier';
import {
	COMMENT_TEXTAREA_PLACEHOLDER,
	MIN_EDITOR_TEXT_HEIGHT,
} from './constants';
import {
	useCommentRenderLeaf,
	useEditorFormHandlers,
	useEditorFromPlate,
} from './hooks';
import {
	CommentEditorWrapper,
	FormControlsWrapper,
	PlateWrapper,
	getPlaceholderStyles,
} from './styled';
import { EditorFormProps } from './types';

export function EditorForm({
	editorId,
	editorData,
	files,
	onSetFiles,
	isSubmitting,
	isEditable,
	mode,
	onEditorChange,
	onSubmit,
	onReset,
	children,
}: EditorFormProps) {
	const theme = useTheme();
	const { setHasUnsavedChanges } = useAbsenceRequestChanges();

	const customPlugins = useMemo(
		() => getCustomPluginsWithThemeMode(theme.mode),
		[theme.mode]
	);

	const { editor, onEditorReady } = useEditorFromPlate(editorId);

	const { handleSubmit, isSendDisabled } = useEditorFormHandlers({
		editor,
		editorData,
		files,
		isSubmitting,
		onSubmit,
		onReset,
		mode,
	});

	const renderLeafNode = useCommentRenderLeaf(editorData);

	const editableProps = useMemo(
		() => ({
			...CONFIG.editableProps,
			placeholder: COMMENT_TEXTAREA_PLACEHOLDER,
			renderLeaf: renderLeafNode,
			readOnly: !isEditable,
			style: {
				...CONFIG.editableProps.style,
				...getPlaceholderStyles({
					theme,
					isEmpty: isEditorEmpty(editorData),
				}),
				height: MIN_EDITOR_TEXT_HEIGHT,
			},
		}),
		[theme, editorData, isEditable, renderLeafNode]
	);

	const hasFiles = files.length > 0;

	const initialEditorDataValue = useMemo(
		() => (editorData.length ? editorData : initialEditorData),
		[editorData]
	);

	useEffect(() => {
		const isEmpty = isEditorEmpty(editorData);
		setHasUnsavedChanges(!isEmpty || hasFiles);
	}, [editorData, hasFiles]);

	return (
		<form onSubmit={handleSubmit}>
			<CommentEditorWrapper hasFiles={hasFiles}>
				{isEditable && editor && (
					<CommentToolbar
						editor={editor}
						files={files}
						onSetFiles={onSetFiles}
					/>
				)}

				<PlateWrapper>
					<Plate
						key={editorId}
						id={editorId}
						value={initialEditorDataValue}
						onChange={onEditorChange}
						plugins={customPlugins}
						editableProps={editableProps}
					>
						<EditorReadyNotifier onReady={onEditorReady} />
						{mode === PlateModes.edit && (
							<EditModeContentSync editorData={editorData} />
						)}
					</Plate>
				</PlateWrapper>
			</CommentEditorWrapper>

			<FormControlsWrapper>{children({ isSendDisabled })}</FormControlsWrapper>
		</form>
	);
}
