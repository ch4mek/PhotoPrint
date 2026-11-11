import { OndemandVideoIcon } from '@mms/mms-ui-library/icons/editor/OndemandVideoIcon';
import {
	getPlateEditorRef,
	insertMediaEmbed,
	MediaEmbedToolbarButton,
} from '@udecode/plate';

import { TOOLBAR_BUTTON_CLASSNAME } from '@/components/PlateToolbars/constants';
import { useToolbarPopUp } from '@/components/PlateToolbars/hooks/useToolbarPopUp';
import { EDITOR_ID } from '@/constants/plateEditor';
import { MediaType } from '@/types/plateEditor';

// eslint-disable-next-line import/no-unused-modules
export function MediaToolbar() {
	const { handleClose, handleButtonClick } = useToolbarPopUp();
	const editor = getPlateEditorRef(EDITOR_ID);

	const handleSubmit = async ({ url }: { url: string }) => {
		if (!editor) {
			return;
		}

		insertMediaEmbed(editor, { url });
		handleClose();
	};

	return (
		<MediaEmbedToolbarButton
			icon={<OndemandVideoIcon />}
			onMouseDown={handleButtonClick(MediaType.Video, handleSubmit)}
			classNames={{ root: TOOLBAR_BUTTON_CLASSNAME, active: '' }}
		/>
	);
}
