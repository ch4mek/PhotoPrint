import { LinkIcon } from '@mms/mms-ui-library/icons/editor/LinkIcon';
import {
	getPlateEditorRef,
	insertLink,
	LinkToolbarButton,
} from '@udecode/plate';
import { BaseEditor, Editor, Node } from 'slate';

import {
	insertsTextTooltips,
	TOOLBAR_BUTTON_CLASSNAME,
} from '@/components/PlateToolbars/constants';
import { useToolbarPopUp } from '@/components/PlateToolbars/hooks/useToolbarPopUp';
import { EDITOR_ID } from '@/constants/plateEditor';
import { MediaType } from '@/types/plateEditor';

export function LinkToolbar() {
	const { handleClose, handleButtonClick } = useToolbarPopUp();
	const editor = getPlateEditorRef(EDITOR_ID);

	const handleSubmit = async ({ url }: { url: string }) => {
		if (!editor) {
			return;
		}

		const baseEditor = editor as BaseEditor;

		const selectedText = editor.selection
			? Node.string({ children: Editor.fragment(baseEditor, editor.selection) })
			: url;

		insertLink(editor, { url, text: selectedText });
		handleClose();
	};

	return (
		<LinkToolbarButton
			icon={<LinkIcon />}
			onMouseDown={handleButtonClick(MediaType.Link, handleSubmit)}
			tooltip={{ content: insertsTextTooltips.link }}
			classNames={{ root: TOOLBAR_BUTTON_CLASSNAME, active: '' }}
		/>
	);
}
