import { FormEvent, useCallback } from 'react';
import { BaseEditor, Editor, Transforms } from 'slate';

import { PlateModes } from '../../../constants';
import { MAX_PLAIN_LENGTH, MIN_PLAIN_LENGTH } from '../constants';
import { UseEditorFormHandlersParams } from '../types';

import { normalizeColorsToLightTheme } from '@/components/PlateToolbars/utils/normalizeColorsToLightTheme';
import { initialEditorData } from '@/constants/plateEditor';
import { normalizeEditorData } from '@/helpers/normalizeEditorData';
import {
	getPlainCharacterCountWithSpaces,
	isEditorEmpty,
} from '@/helpers/plain';

export function useEditorFormHandlers({
	editor,
	editorData,
	files,
	isSubmitting,
	onSubmit,
	onReset,
	mode,
}: UseEditorFormHandlersParams) {
	const plainTextLength = getPlainCharacterCountWithSpaces(editorData);
	const hasFiles = files.length > 0;
	const hasValidText =
		!isEditorEmpty(editorData) &&
		plainTextLength >= MIN_PLAIN_LENGTH &&
		plainTextLength <= MAX_PLAIN_LENGTH;
	const canSend = hasFiles || hasValidText;

	const isSendDisabled = isSubmitting || !canSend;

	const handleReset = useCallback(() => {
		onReset();

		if (editor) {
			editor.children = initialEditorData;
			editor.selection = null;

			Transforms.deselect(editor as BaseEditor);
			Editor.normalize(editor as BaseEditor, { force: true });
		}
	}, [onReset, editor]);

	const handleSubmit = useCallback(
		async (e: FormEvent) => {
			e.preventDefault();

			if (isSendDisabled || !editor) {
				return;
			}

			const normalized = normalizeColorsToLightTheme(
				normalizeEditorData(editorData)
			);
			const text = JSON.stringify(normalized);

			await onSubmit(text, files);

			if (mode === PlateModes.create) {
				handleReset();
			}
		},
		[editor, editorData, files, isSendDisabled, onSubmit, mode, handleReset]
	);

	return { handleReset, handleSubmit, isSendDisabled };
}
