import { FormikErrors, FormikTouched } from 'formik';

import { OutsideCloseHandler } from '@/forms/components/FormikWithBackup/types';
import { UnknownObject } from '@/types/common';
import { SimpleConfigurableConfigurationItem } from '@/types/configuration';
import { Document } from '@/types/document';

export type RequestDocumentsFormViewProps = {
	values: {
		documentType: SimpleConfigurableConfigurationItem<string | number> | null;
		files: Document[];
		notes: string | null;
	};
	errors: FormikErrors<{
		documentType: SimpleConfigurableConfigurationItem<string | number> | null;
		files: Document[];
		notes: string | null;
	}>;
	touched: FormikTouched<{
		documentType: SimpleConfigurableConfigurationItem<string | number> | null;
		files: Document[];
		notes: string | null;
	}>;
	userId: number | undefined;
	isShowLoader: boolean;
	popUpStylesConfig: UnknownObject;
	documentHandlers: {
		documents: Document[];
		handleFileChange: (
			e: React.ChangeEvent<HTMLInputElement>,
			currentFilesCount: number
		) => File[] | undefined;
		handleFileRemove: (fileId: string | number) => void;
		resetDocuments: () => void;
	};
	documentTypeOptions: SimpleConfigurableConfigurationItem<string | number>[];
	handleDocumentTypeChange: (
		option: SimpleConfigurableConfigurationItem<string | number> | null
	) => void;
	handleClose: VoidFunction;
	handlePositiveControl: VoidFunction;
} & OutsideCloseHandler;
