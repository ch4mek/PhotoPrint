import { useCallback, useState, useEffect, useRef } from 'react';

import { initialEditorData } from '@/constants/plateEditor';
import { AbsenceRequestChangesProvider } from '@/forms/Absence/ViewAbsenceRequestForm/context/AbsenceRequestChangesContext';
import { AppFile } from '@/types/file';
import { CustomValue } from '@/types/plateEditor';

import { AttachedFilesList } from './components/AttachedFilesList';
import { EditorForm } from './components/EditorForm';
import { PlateModes } from './constants';
import { CommentEditorProps } from './types';

export function CommentEditor({
	onSubmit,
	editorId,
	children,
	initialValue = initialEditorData,
	initialFiles = [],
	isSubmitting = false,
	isEditable = true,
	mode = PlateModes.create,
}: CommentEditorProps) {
	const [files, setFiles] = useState<AppFile[]>(initialFiles);
	const [editorData, setEditorData] = useState<CustomValue>(
		initialValue.length ? initialValue : initialEditorData
	);
	const prevEditorIdRef = useRef<string | null>(null);

	useEffect(() => {
		if (prevEditorIdRef.current !== editorId) {
			if (mode === PlateModes.edit) {
				setEditorData(initialValue.length ? initialValue : initialEditorData);
				setFiles(initialFiles);
			}
			prevEditorIdRef.current = editorId;
		}
	}, [editorId, initialValue, initialFiles, mode]);

	const handleAttachedFileRemove = useCallback(
		(removedFileId: string | number) => {
			setFiles((prevState) =>
				prevState.filter(({ fileId }) => fileId !== removedFileId)
			);
		},
		[]
	);

	const handleReset = useCallback(() => {
		setEditorData(initialEditorData);
		setFiles([]);
	}, []);

	const handleEditorChange = useCallback((value: CustomValue) => {
		setEditorData(value);
	}, []);

	return (
		<AbsenceRequestChangesProvider>
			<div>
				<AttachedFilesList
					files={files}
					onFileRemove={handleAttachedFileRemove}
				/>

				<EditorForm
					{...{
						editorId,
						editorData,
						files,
						isSubmitting,
						isEditable,
						mode,
						onSubmit,
					}}
					onSetFiles={setFiles}
					onEditorChange={handleEditorChange}
					onReset={handleReset}
				>
					{children}
				</EditorForm>
			</div>
		</AbsenceRequestChangesProvider>
	);
}
