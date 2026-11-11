import { Plate } from '@udecode/plate';

import { FilesGroup } from '../../../../../FilesGroup';

import { isEditorEmpty } from '@/helpers/plain';
import { CONFIG } from '@/plugins/PlateEditorPlugins/config';
import { UserInquiryFiles } from '@/types/absences';
import { CustomValue } from '@/types/plateEditor';

import { CommentContentProps } from './types';

export function CommentContent({
	files,
	plugins,
	value,
	editorId,
}: CommentContentProps) {
	const hasText = !isEditorEmpty(value);

	return (
		<>
			{hasText && (
				<Plate<CustomValue>
					key={editorId}
					id={editorId}
					{...{ value, plugins }}
					editableProps={{
						...CONFIG.editableProps,
						readOnly: true,
					}}
				/>
			)}

			{!!files.length && <FilesGroup data={files as UserInquiryFiles[]} />}
		</>
	);
}
