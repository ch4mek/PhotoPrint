import { ImageIcon } from '@mms/mms-ui-library/icons/editor/ImageIcon';
import {
	getPlateEditorRef,
	ImageToolbarButton,
	insertImage,
} from '@udecode/plate';

import {
	insertsTextTooltips,
	TOOLBAR_BUTTON_CLASSNAME,
} from '@/components/PlateToolbars/constants';
import { useToolbarPopUp } from '@/components/PlateToolbars/hooks/useToolbarPopUp';
import { EDITOR_ID } from '@/constants/plateEditor';
import { MediaType } from '@/types/plateEditor';

export function ImageToolbar() {
	const { handleClose, handleButtonClick } = useToolbarPopUp();
	const editor = getPlateEditorRef(EDITOR_ID);

	const handleSubmit = async ({ url }: { url: string }) => {
		if (!editor) {
			return;
		}

		await insertImage(editor, url);
		handleClose();
	};

	return (
		<ImageToolbarButton
			icon={<ImageIcon />}
			onMouseDown={handleButtonClick(MediaType.Image, handleSubmit)}
			tooltip={{ content: insertsTextTooltips.image }}
			classNames={{ root: TOOLBAR_BUTTON_CLASSNAME, active: '' }}
		/>
	);
}
