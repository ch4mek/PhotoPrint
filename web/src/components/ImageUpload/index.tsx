import { ActionButton } from '@mms/mms-ui-library/components/ActionButton';
import { BasketIcon } from '@mms/mms-ui-library/icons/base/BasketIcon';
import { ChangeEvent, useCallback, useRef, useState } from 'react';

import { Upload } from '@/components/ImageUpload/components/Upload';
import {
	DEFAULT_UPLOADED_IMAGE,
	UPLOAD_SIZE,
} from '@/components/ImageUpload/constants';
import {
	HiddenInput,
	ImageUploadWrapper,
	UploadWrapper,
} from '@/components/ImageUpload/styles';
import { ImageUploadProps } from '@/components/ImageUpload/types';
import { BorderRadiuses } from '@/constants/borderRadiuses';
import { imageValidExtensions } from '@/constants/fileExtensions';
import { formatAllowedFileTypesToAccept } from '@/helpers/formatAllowedFileTypes';

export function ImageUpload({
	alt = DEFAULT_UPLOADED_IMAGE,
	size = UPLOAD_SIZE,
	uploadRadius = BorderRadiuses.m,
	file,
	removeFile,
	handleFileChange,
	disabled,
}: ImageUploadProps) {
	const [isHintOpen, setIsHintOpen] = useState(false);

	const uploadRef = useRef<HTMLDivElement>(null);
	const hiddenFileInput = useRef<HTMLInputElement>(null);

	const handleUploadClick = useCallback(() => {
		hiddenFileInput.current?.click();
	}, []);

	const handleImageChange = (event: ChangeEvent<HTMLInputElement>) => {
		setIsHintOpen(false);
		handleFileChange(event);
	};

	return (
		<ImageUploadWrapper>
			<UploadWrapper>
				<Upload
					{...{
						file,
						handleUploadClick,
						setIsHintOpen,
						isHintOpen,
						alt,
						size,
						uploadRadius,
					}}
					ref={uploadRef}
					{...{
						file,
						handleUploadClick,
						setIsHintOpen,
						isHintOpen,
						alt,
						size,
						uploadRadius,
						disabled,
					}}
				/>

				{file && (
					<ActionButton onClick={removeFile} {...{ disabled }}>
						<BasketIcon />
					</ActionButton>
				)}
			</UploadWrapper>

			<HiddenInput
				{...{ disabled }}
				type="file"
				ref={hiddenFileInput}
				onChange={handleImageChange}
				accept={formatAllowedFileTypesToAccept(imageValidExtensions)}
				{...{ disabled }}
				hidden
			/>
		</ImageUploadWrapper>
	);
}
