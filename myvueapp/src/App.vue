<template>
  <div class="container">
    <ejs-uploader :asyncSettings="asyncSettings" :removing="onFileRemoving" 
      name="UploadFiles" :uploading="onFileUploading"></ejs-uploader>
  </div>
</template>

<script>
import { UploaderComponent } from '@syncfusion/ej2-vue-inputs';

export default {
  name: 'App',
  components: {
    'ejs-uploader': UploaderComponent
  },
  data() {
    return {
      asyncSettings: {
        saveUrl: 'https://localhost:7113/Home/Save',
        removeUrl: 'https://localhost:7113/Home/Remove',
      },
      token: 'Your.JWT.Token'
    };
  },
  methods: {
    onFileUploading(args){
      args.currentRequest.setRequestHeader('Authorization',`Bearer ${this.token}`);
    },
    onFileRemoving(args) {
      args.postRawFile = false;
      args.currentRequest.setRequestHeader('Authorization',`Bearer ${this.token}`);
    }
  }
};
</script>
<style>
@import "../node_modules/@syncfusion/ej2-vue-inputs/styles/material.css";
@import "../node_modules/@syncfusion/ej2-base/styles/material.css";
@import "../node_modules/@syncfusion/ej2-buttons/styles/material.css";

.container {
  width: 400px;
  position: absolute;
  top: 15%;
  left: 40%;
}
</style>